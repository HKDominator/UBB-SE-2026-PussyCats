using System;
using System.IO;

namespace PussyCatsApp.Services
{
    public class LocalFileStorageService : ILocalFileStorageService
    {
        private string basePath = Path.Combine("uploads", "documents");

        public LocalFileStorageService()
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, basePath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        public LocalFileStorageService(string basePath)
        {
            this.basePath = basePath;

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        public string SaveFile(Stream fileStream, string originalFileName)
        {
            string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
            string savedFileFullPath = Path.IsPathRooted(basePath)
                ? Path.Combine(basePath, uniqueFileName)
                : Path.Combine(AppContext.BaseDirectory, basePath, uniqueFileName);

            using var fileOutputStream = File.Create(savedFileFullPath);

            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }
            fileStream.CopyTo(fileOutputStream);

            return savedFileFullPath;
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }
            string resolvedFileFullPath = Path.Combine(AppContext.BaseDirectory, basePath, Path.GetFileName(relativePath));
            if (!Path.IsPathRooted(resolvedFileFullPath))
            {
                resolvedFileFullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
            }
            if (File.Exists(resolvedFileFullPath))
            {
                File.Delete(resolvedFileFullPath);
            }
        }

        public string GetFilePath(string relativePath)
        {
            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }
            string returnedPath = Path.Combine(AppContext.BaseDirectory, relativePath);
            if (!Path.Exists(returnedPath))
            {
                throw new FileNotFoundException($"File not found at path: {relativePath}");
            }
            return returnedPath;
        }
    }
}