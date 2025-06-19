using System;
using Core.Attributes;

namespace Core.Entities
{
    [DbContextName("FirstDbContext")]
    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
