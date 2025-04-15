using System.ComponentModel.DataAnnotations;

namespace ASPWebApp.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
