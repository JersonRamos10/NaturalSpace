using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Guid messageId, IFormFile file);
        Task DeleteFileAsync(string filePath);
    }
}
