using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogStore.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "The user name is required")]
        [MaxLength(100, ErrorMessage = "user name cannot exceed 100 characters")]
        public string UserName { get; set; }
        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; }
        [Required]
        public string Content { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

    }
}
