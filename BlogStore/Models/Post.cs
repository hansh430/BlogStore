using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogStore.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(400, ErrorMessage = "Title cannot exceed 400 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
        public string? Url { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Length cannot exceed 100 characters")]
        public string Author { get; set; }
        public string FeatureImagePath { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
