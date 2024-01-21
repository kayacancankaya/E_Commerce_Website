using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IakademiWebProject.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Ürün Adı Zorunlu Alan")]
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        [DisplayName("Kategori")]
        public int CategoryID { get; set; }
        [DisplayName("Marka")]
        public int SupplierID { get; set; }
		[DisplayName("Stok")]
		public int Stock { get; set; }
		[DisplayName("İndirim")]
		public int Discount { get; set; } 
        [DisplayName("Statüs")]
        public int StatusID { get; set; }
		[DisplayName("Eklenme Tarihi")]
		public DateTime AddDate { get; set; }
		[DisplayName("Anahtar Kelimeler")]
		public string? Keywords { get; set; }

        private int _Kdv;

        public int Kdv
        {
            get { return _Kdv; }
            set { _Kdv = Math.Abs(value); }
        }
		[DisplayName("Öne Çıkan")]
		public int HighLighted { get; set; }
		[DisplayName("Çok Satan")]
		public int TopSeller { get; set; }
		[DisplayName("Alakalı")]
		public int Related { get; set; }
		[DisplayName("Notlar")]
		public string? Notes { get; set; }
		[DisplayName("Resim Yolu")]
		public string? PhotoPath { get; set; }
		[DisplayName("Aktif")]
		public bool Active { get; set; }

    }
}
