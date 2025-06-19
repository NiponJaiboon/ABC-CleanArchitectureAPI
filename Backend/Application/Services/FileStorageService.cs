using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class FileStorageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly string _storagePath;

        public FileStorageService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;

            _storagePath = config["StoragePath"] ?? "Storage";
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subDir, string userId = null)
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var timePart = DateTime.UtcNow.ToString("HHmmssfff");
            var userPart = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            var fileType = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant();
            var fileCategory = GetFileCategory(fileType);
            var targetDir = Path.Combine(_storagePath, fileCategory, datePart, userPart);
            var fileName = $"{timePart}{Path.GetExtension(file.FileName)}";

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            var filePath = Path.Combine(targetDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var userFile = new UserFile
            {
                UserId = userId,
                FileName = file.FileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                UploadDate = DateTime.UtcNow,
            };
            await _unitOfWork.Repository<UserFile>().AddAsync(userFile);
            await _unitOfWork.CommitAsync();

            return filePath;
        }

        private static string GetFileCategory(string fileType)
        {
            string fileCategory;
            var imageExtensions = new[] { "jpg", "jpeg", "png", "gif", "bmp", "tiff", "webp" };
            var documentExtensions = new[]
            {
                "pdf",
                "doc",
                "docx",
                "xls",
                "xlsx",
                "ppt",
                "pptx",
                "txt",
                "rtf",
            };

            if (imageExtensions.Contains(fileType))
            {
                fileCategory = "image";
            }
            else if (documentExtensions.Contains(fileType))
            {
                fileCategory = "document";
            }
            else
            {
                fileCategory = "other";
            }

            return fileCategory;
        }
    }
}
