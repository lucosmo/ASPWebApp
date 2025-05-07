using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ASPWebApp.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Body { get; set; }
        [BindProperty(Name = "author")]
        public int UserId { get; set; }
        [ValidateNever]
        public User Author { get; set; }
    }
}
