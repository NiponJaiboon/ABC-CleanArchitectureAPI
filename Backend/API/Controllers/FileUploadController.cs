using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Application.Services;
    using Core.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    namespace YourNamespace.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class FileUploadController : ControllerBase
        {
            private readonly ILogger<FileUploadController> _logger;
            private readonly FileStorageService _fileStorageService;

            public FileUploadController(
                ILogger<FileUploadController> logger,
                FileStorageService fileStorageService
            )
            {
                _fileStorageService = fileStorageService;
                _logger = logger;
            }

            private readonly string _storagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Storage"
            );

            [Authorize]
            [HttpPost("upload")]
            [RequestSizeLimit(10 * 1024 * 1024)] // Limit to 10 MB ต่อไฟล์
            public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
            {
                try
                {
                    var userId = User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier
                    )?.Value;

                    try
                    {
                        var uploadedFiles = new List<string>();
                        foreach (var file in files)
                        {
                            var path = await _fileStorageService.SaveFileAsync(
                                file,
                                "uploads",
                                userId
                            );
                            uploadedFiles.Add(path);
                        }
                        return Ok(new { uploadedFiles });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred during file upload.");
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during file upload.");
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            private static string GetSubDirectoryByFileType(IFormFile file)
            {
                // Check file type and create subdirectory by type
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                string subDir = "Others";
                if (
                    extension == ".jpg"
                    || extension == ".jpeg"
                    || extension == ".png"
                    || extension == ".gif"
                )
                {
                    subDir = "Images";
                }
                else if (extension == ".pdf")
                {
                    subDir = "Documents";
                }
                else if (extension == ".mp4" || extension == ".avi" || extension == ".mov")
                {
                    subDir = "Videos";
                }

                return subDir;
            }
        }
    }
}
