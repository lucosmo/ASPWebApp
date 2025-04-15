using System.ComponentModel.DataAnnotations;

namespace ASPWebApp.Models
{
    public class UsedImage
    {
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
