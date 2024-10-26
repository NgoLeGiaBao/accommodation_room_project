using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace App.Models.NewsModel
{
    public class ContentNews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Post title is required")]
        [Display(Name = "General Title")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        public string GeneralTitle { get; set; }

        [Required(ErrorMessage = "Post title is required")]
        [Display(Name = "Detail Title")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        public string DetailTitle { get; set; }

        [Display(Name = "Detail Description")]
        public string GeneralDescription { get; set; }

        [Display(Name = "Detail Description")]
        public string DetailDescription { get; set; }

        [Display(Name = "Identifier String (URL)", Prompt = "Enter or leave blank to auto-generate from Title")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Only characters [a-z0-9-] are allowed")]
        public string? Slug { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }
        [Display(Name = "Published")]
        public bool Published { get; set; }
        public List<CategoryNews>? CategoryNewses { get; set; }

        //[Required]
        [Display(Name = "Author")]
        public string? AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        [Display(Name = "Author")]
        public AppUser? Author { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Update Date")]
        public DateTime DateUpdated { get; set; }
        [Display(Name = "ImageId")]
        public string? ImageId { get; set; }
        [Display(Name = "Keyword")]
        public string KeyWord { get; set; }
        public List<ContentAndCategoryNews>? PostCategories { get; set; }
    }
}