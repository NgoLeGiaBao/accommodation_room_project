using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    [Table("Contract")]
    public class Contract
    {
        [Key]
        public int Id { get; set; }


    }
}
