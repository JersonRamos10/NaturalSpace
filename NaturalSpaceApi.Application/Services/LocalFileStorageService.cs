using Microsoft.AspNetCore.Http;
using NaturalSpaceApi.Application.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NaturalSpaceApi.Application.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _uploadsPath;
        private readonly string _baseUrl;

        public LocalFileStorageService()
        {
            _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            _baseUrl = "/uploads";
        }

        public async Task<string> SaveFileAsync(Guid messageId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var messageFolder = Path.Combine(_uploadsPath, messageId.ToString());
            if (!Directory.Exists(messageFolder))
            {
                Directory.CreateDirectory(messageFolder);
            }

            var safeFileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(messageFolder, safeFileName);

            // Si ya existe, agregar un sufijo numérico
            int counter = 1;
            var originalFileName = Path.GetFileNameWithoutExtension(safeFileName);
            var extension = Path.GetExtension(safeFileName);
            while (File.Exists(filePath))
            {
                var newFileName = $"{originalFileName}_{counter}{extension}";
                filePath = Path.Combine(messageFolder, newFileName);
                counter++;
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"{_baseUrl}/{messageId}/{Path.GetFileName(filePath)}";
            return relativePath;
        }

        public Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }
    }
}
