using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace App.Models.NewsModel
{
    public class CategoryNews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        // Parent Category (Foreign Key)
        [Display(Name = "Parent Category")]
        public string? ParentCategoryId { get; set; }

        // Category Title
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        [Display(Name = "Category Name")]
        public string Title { get; set; }

        // Content, detailed information about the Category
        [DataType(DataType.Text)]
        [Display(Name = "Category Content")]
        public string Description { get; set; }

        // URL slug
        [Required(ErrorMessage = "URL is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Only characters [a-z0-9-] are allowed")]
        [Display(Name = "Display URL")]
        public string Slug { get; set; }

        // Child Categories
        public ICollection<CategoryNews>? CategoryChildren { get; set; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Parent Category")]
        public CategoryNews? ParentCategory { get; set; }

        public ICollection<ContentAndCategoryNews>? PostCategories { get; set; }

        // Retrieve all child categories of a category
        public void ChildCategoryIDs(ICollection<CategoryNews> childCates, List<string> list)
        {
            if (childCates == null)
                childCates = this.CategoryChildren;
            foreach (CategoryNews category in childCates)
            {
                list.Add(category.Id);
                ChildCategoryIDs(category.CategoryChildren, list);
            }
        }

        // Retrieve all parent categories of a category
        public List<CategoryNews> GetListParents()
        {
            List<CategoryNews> li = new List<CategoryNews>();
            var parent = this.ParentCategory;
            while (parent != null)
            {
                li.Add(parent);
                parent = parent.ParentCategory;
            }
            li.Reverse();
            return li;
        }
    }
}
