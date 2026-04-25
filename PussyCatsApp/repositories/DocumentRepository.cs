using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly string connectionString;

        public DocumentRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Document> GetDocumentsByUserId(int userId)
        {
            var documents = new List<Document>();

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string selectDocumentByUserIdQuery = @"
                SELECT dID         AS Id,
                       userID      AS UserId,
                       nameDocument AS DocumentName,
                       FilePath,
                       UploadDate
                FROM   DOCUMENTS
                WHERE  userID = @UserId";

                using var command = new SqlCommand(selectDocumentByUserIdQuery, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Document document = MapRowToDocument(reader);
                    documents.Add(document);
                }
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error retrieving documents for user {userId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred retrieving documents for user {userId}: {ex.Message}");
            }

            return documents;
        }

        public Document GetDocumentById(int documentId)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string selectDocumentByIdQuery = @"
                SELECT dID         AS Id,
                       userID      AS UserId,
                       nameDocument AS DocumentName,
                       FilePath,
                       UploadDate
                FROM   DOCUMENTS
                WHERE  dID = @Id";

                using var command = new SqlCommand(selectDocumentByIdQuery, connection);
                command.Parameters.AddWithValue("@Id", documentId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Document document = MapRowToDocument(reader);
                    return document;
                }
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error retrieving document with ID {documentId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred retrieving document with ID {documentId}: {ex.Message}");
            }
            return null;
        }

        public void AddDocument(Document document)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string addDocumentQuery = @"
                INSERT INTO DOCUMENTS (userID, nameDocument, FilePath, UploadDate)
                VALUES (@UserId, @DocumentName, @FilePath, @UploadDate)";

                using var command = new SqlCommand(addDocumentQuery, connection);
                command.Parameters.AddWithValue("@UserId", document.UserId);
                command.Parameters.AddWithValue("@DocumentName", document.DocumentName);
                if (document.FilePath == null)
                {
                    command.Parameters.AddWithValue("@FilePath", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@FilePath", document.FilePath);
                }
                command.Parameters.AddWithValue("@UploadDate", document.UploadDate);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error adding document for user {document.UserId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred adding document for user {document.UserId}: {ex.Message}");
            }
        }
        public void DeleteDocument(int documentId)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                const string deleteDocumentQuery = "DELETE FROM DOCUMENTS WHERE dID = @Id";

                using var command = new SqlCommand(deleteDocumentQuery, connection);
                command.Parameters.AddWithValue("@Id", documentId);
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error deleting document with ID {documentId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred deleting document with ID {documentId}: {ex.Message}");
            }
        }

        private static Document MapRowToDocument(SqlDataReader reader)
        {
            try
            {
                Document document = new Document();
                document.Id = Convert.ToInt32(reader["Id"]);
                document.UserId = Convert.ToInt32(reader["UserId"]);

                if (reader["DocumentName"] == DBNull.Value)
                {
                    document.DocumentName = string.Empty;
                }
                else
                {
                    document.DocumentName = reader["DocumentName"].ToString();
                }

                if (reader["FilePath"] == DBNull.Value)
                {
                    document.FilePath = null;
                }
                else
                {
                    document.FilePath = reader["FilePath"].ToString();
                }

                if (reader["UploadDate"] == DBNull.Value)
                {
                    document.UploadDate = DateTime.MinValue;
                }
                else
                {
                    document.UploadDate = Convert.ToDateTime(reader["UploadDate"]);
                }

                return document;
            }
            catch (InvalidCastException castEx)
            {
                throw new Exception("Mapping error.", castEx);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
 }