
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace IakademiWebProject.Models
{
    public class Status
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatusID { get; set; }
		[DisplayName("Durum Adı")]
		[StringLength(100)]
        [Required]
        public string? StatusName { get; set; }
		[DisplayName("Aktif Mi")]
		public bool Active { get; set; }
    }
}
