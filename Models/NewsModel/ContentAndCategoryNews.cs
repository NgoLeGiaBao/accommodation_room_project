using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace App.Models.NewsModel
{
    public class ContentAndCategoryNews
    {
        public string categoryNewsID { set; get; }

        public string contentNewsID { set; get; }

        [ForeignKey("CategoryNewsID")]
        public CategoryNews categoryNews { set; get; }

        [ForeignKey("ContentID")]
        public ContentNews contentNews { set; get; }
    }
}