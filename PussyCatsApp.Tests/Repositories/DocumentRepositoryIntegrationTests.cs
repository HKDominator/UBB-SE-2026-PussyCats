using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class DocumentRepositoryIntegrationTests
    {
        private DocumentRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new DocumentRepository(TestDatabaseHelper.ConnectionString);
        }

        [TestMethod]
        public void GetDocumentsByUserId_UserHasTwoDocuments_ExpectsDocumentsBeingReturnedInOrder()
        {
            string FirstDocumentName = "Test Document 1";
            string SecondDocumentName = "Test Document 2";
            int SecondDocumentIndex = 1;

            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertDocument(userId, FirstDocumentName);
            TestDatabaseHelper.InsertDocument(userId, SecondDocumentName);

            List<Document> documents = Repository.GetDocumentsByUserId(userId);

            Assert.AreEqual(SecondDocumentName, documents[SecondDocumentIndex].DocumentName);
        }

        [TestMethod]
        public void GetDocumentsByUserId_InvalidServer_ExpectsNoDocument()
        {
            int DummyUserId = 1;
            int ExpectedZeroCount = 0;
            string InvalidConnectionString = "Server=InvalidServerName;Database=Fake;Connect Timeout=1;";

            var repository = new DocumentRepository(InvalidConnectionString);

            List<Document> result = repository.GetDocumentsByUserId(DummyUserId);

            Assert.AreEqual(ExpectedZeroCount, result.Count);
        }

        [TestMethod]
        public void GetDocumentById_InvalidServer_ExpectsNoDocument()
        {
            int DummyDocumentId = 1;
            string InvalidConnectionString = "Server=InvalidServerName;Database=Fake;Connect Timeout=1;";

            var repository = new DocumentRepository(InvalidConnectionString);

            Document result = repository.GetDocumentById(DummyDocumentId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void AddDocument_InvalidServer_ExpectsNotCrashing()
        {
            int DummyUserId = 1;
            string InvalidConnectionString = "Server=InvalidServerName;Database=Fake;Connect Timeout=1;";

            var repository = new DocumentRepository(InvalidConnectionString);

            repository.AddDocument(new Document { UserId = DummyUserId });
        }

        [TestMethod]
        public void DeleteDocument_InvalidServer_ExpectsNoCrashing()
        {
            int DummyDocumentId = 1;
            string InvalidConnectionString = "Server=InvalidServerName;Database=Fake;Connect Timeout=1;";

            var repository = new DocumentRepository(InvalidConnectionString);

            repository.DeleteDocument(DummyDocumentId);
        }

        [TestMethod]
        public void AddDocument_ValidDocument_ExpectsDocumentBeingSaved()
        {
            string TargetDocumentName = "Test Document";
            int FirstDocumentIndex = 0;

            int userId = TestDatabaseHelper.InsertUser();
            Document document = new Document
            {
                UserId = userId,
                DocumentName = TargetDocumentName,
                UploadDate = DateTime.Now
            };
            Repository.AddDocument(document);

            List<Document> documents = Repository.GetDocumentsByUserId(userId);

            Assert.AreEqual(TargetDocumentName, documents[FirstDocumentIndex].DocumentName);
        }

        [TestMethod]
        public void GetDocumentById_UserHasOneDocument_ExpectsDocumentBeingReturned()
        {
            string TargetDocumentName = "Test Document";

            int userId = TestDatabaseHelper.InsertUser();
            int documentId = TestDatabaseHelper.InsertDocument(userId, TargetDocumentName);

            Document document = Repository.GetDocumentById(documentId);

            Assert.AreEqual(TargetDocumentName, document.DocumentName);
        }

        [TestMethod]
        public void DeleteDocument_UserHasOneDocument_ExpectsDocumentBeingDeleted()
        {
            string TargetDocumentName = "Test Document";

            int userId = TestDatabaseHelper.InsertUser();
            int documentId = TestDatabaseHelper.InsertDocument(userId, TargetDocumentName);

            Repository.DeleteDocument(documentId);
            Document document = Repository.GetDocumentById(documentId);

            Assert.IsNull(document);
        }

        [TestMethod]
        public void MapRowToDocument_NullFilePath_ExpectsNull()
        {
            string DocumentName = "NoPath.pdf";
            string NullFilePath = null;

            int userId = TestDatabaseHelper.InsertUser();
            int documentId = TestDatabaseHelper.InsertDocument(userId, DocumentName, NullFilePath);

            Document result = Repository.GetDocumentById(documentId);

            Assert.IsNull(result.FilePath);
        }

        [TestMethod]
        public void MapRowToDocument_NullUploadDate_ExpectsMinValue()
        {
            string DocumentName = "NoDate.pdf";
            string NullFilePath = null;

            int userId = TestDatabaseHelper.InsertUser();
            int documentId = TestDatabaseHelper.InsertDocument(userId, DocumentName, NullFilePath);

            Document result = Repository.GetDocumentById(documentId);

            Assert.AreEqual(DateTime.MinValue, result.UploadDate);
        }

        [TestMethod]
        public void GetDocumentsByUserId_MalformedConnectionString_ExpectsEmptyList()
        {
            int DummyUserId = 1;
            int ExpectedZeroCount = 0;
            string EmptyConnectionString = "";

            var repositoryWithGeneralError = new DocumentRepository(EmptyConnectionString);

            List<Document> result = repositoryWithGeneralError.GetDocumentsByUserId(DummyUserId);

            Assert.AreEqual(ExpectedZeroCount, result.Count, "Should return an empty list after catching a general exception.");
        }

        [TestMethod]
        public void GetDocumentById_MalformedConnectionString_ExpectsNullResult()
        {
            int DummyDocumentId = 1;
            string EmptyConnectionString = "";

            var repositoryWithGeneralError = new DocumentRepository(EmptyConnectionString);

            Document result = repositoryWithGeneralError.GetDocumentById(DummyDocumentId);

            Assert.IsNull(result, "Should return null after catching a general exception.");
        }

        [TestMethod]
        public void AddDocument_MalformedConnectionString_ExpectsErrorBeingHandled()
        {
            int DummyUserId = 1;
            string EmptyConnectionString = "";
            string DocumentName = "Test";

            var repositoryWithGeneralError = new DocumentRepository(EmptyConnectionString);
            Document dummyDocument = new Document { UserId = DummyUserId, DocumentName = DocumentName };

            repositoryWithGeneralError.AddDocument(dummyDocument);
        }

        [TestMethod]
        public void DeleteDocument_MalformedConnectionString_CatchesGeneralException()
        {
            int DummyDocumentId = 1;
            string EmptyConnectionString = "";

            var repositoryWithGeneralError = new DocumentRepository(EmptyConnectionString);
            repositoryWithGeneralError.DeleteDocument(DummyDocumentId);
        }

        [TestMethod]
        public void MapRowToDocument_ValidFilePath_ExpectsSetPathString()
        {
            string ExpectedPath = "C:\\Documents\\test.pdf";
            string DocumentName = "TestDocument";

            int userId = TestDatabaseHelper.InsertUser();
            int documentId = TestDatabaseHelper.InsertDocument(userId, DocumentName, ExpectedPath);

            Document result = Repository.GetDocumentById(documentId);

            Assert.AreEqual(ExpectedPath, result.FilePath);
        }
    }
}