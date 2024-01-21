using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IakademiWebProject.Models
{
    public class Supplier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierID { get; set; }

		[DisplayName("Marka")]
		[StringLength(100)]
        [Required]
        public string? BrandName { get; set; }
        [DisplayName("Resim")]
        public string? PhotoPath { get; set; }

        public bool Active { get; set; }
    }
}
