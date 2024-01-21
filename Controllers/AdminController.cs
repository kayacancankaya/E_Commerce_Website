using IakademiWebProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Data;
using System.Drawing;
using System.Threading.Channels;
using XAct;

namespace IakademiWebProject.Controllers
{
	public class AdminController : Controller
	{
		Cls_User u = new ();
		Cls_Supplier s = new ();
		Cls_Status st = new ();
		Cls_Category c = new();
        Cls_Product p = new();
        iakademi38Context _context = new ();
		public IActionResult Login()
		{
			return View();
		}
		

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login([Bind("Email,Password,NameSurname")] User user)
		{
			if (ModelState.IsValid)
			{
				User? usr = await u.LoginControl(user);

				if (usr != null)
				{
					return RedirectToAction("Index");
				}
			}
			else
			{
				ViewBag.error = "Login ve/veya şifre yanlış"; 
			}
			return View();
		}

		public IActionResult Index() 
		{ 
			return View();
		}
		public async Task<IActionResult> CategoryIndex() 
		{
			List<Category> categories = await c.CategorySelect(); 
			return View(categories);
		}
		public IActionResult CategoryCreate() 
		{
			CategoryFill();
			return View();
		}
		public void CategoryFill() 
		{
			List<SelectListItem> distinctCategoryItems = _context.Categories
								.Select(item => new { item.CategoryName, item.CategoryID })
								.Distinct()
								.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() })
								.ToList();

			ViewData["categoryList"] = distinctCategoryItems;

				//categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });

		}

		[HttpPost]
		public IActionResult CategoryCreate(Category category)
		{
			bool answer = Cls_Category.CategoryInsert(category);
			TempData["Message"] = answer ?  "Eklendi" : "Hata";

			return RedirectToAction(nameof(CategoryCreate));

		}

		public async Task<IActionResult> CategoryEdit(int? id)
		{
			CategoryFill();
			if(id == null || _context.Categories == null)
			{
				return NotFound();
			}
			var category = await c.CategoryDetails(id);
			return View(category);
		}
		[HttpPost]
		public IActionResult CategoryEdit(Category category) 
		{
			bool answer = Cls_Category.CategoryUpdate(category);
			TempData["Message"] = answer ? "Güncellendi" : "Hata";

			return RedirectToAction("CategoryIndex");

		}
		[HttpPost]
		public IActionResult CategoryUpdate(Category category) 
		{
			bool answer = Cls_Category.CategoryUpdate(category);
			TempData["Message"] = answer ? "Güncellendi" : "Hata";

			return RedirectToAction("CategoryIndex");

		}
		[HttpGet]
		public async Task<IActionResult> CategoryDelete(int? id) 
		{            
			if(id == null || _context.Categories == null) 
				return NotFound();

			var category = await _context.Categories.FirstOrDefaultAsync(c=>c.CategoryID == id);

			if(category == null)
				return NotFound();

			return View(category);

		}

        [HttpPost,ActionName("CategoryDelete")]
        public async Task<IActionResult> CategoryDeleteConfirmed(int id)
        {
            bool answer = await Cls_Category.CategoryDelete(id);

			if(answer)
			{ 
            TempData["Message"] = "Silindi" ;
            return RedirectToAction("CategoryIndex");
            }
			else
			{ 
            TempData["Message"] = "Hata" ;
            return RedirectToAction("CategoryDelete");
            }

        }
        public async Task<IActionResult> CategoryDetails(int? id)
		{
			var category = await _context.Categories.FirstOrDefaultAsync(c=>c.CategoryID == id);
			ViewBag.categoryname = category?.CategoryName;
			return View(category);
		}

        public async Task<IActionResult> SupplierIndex()
        {
            List<Supplier> suppliers = await s.SupplierSelect();
            return View(suppliers);
        }


		public IActionResult SupplierCreate()
		{
			return View();
		}

		[HttpPost]
		public IActionResult SupplierCreate(Supplier supplier)
		{
			bool answer = Cls_Supplier.SupplierInsert(supplier);
           
			TempData["Message"] = answer ? supplier.BrandName + " Markası Eklendi." : "HATA " + supplier.BrandName + " Markası Eklenemedi.";

			return RedirectToAction(nameof(SupplierCreate));

        }
        public async Task<IActionResult> SupplierDetails(int? id)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(c => c.SupplierID == id);
            ViewBag.suppliername = supplier?.BrandName;
            return View(supplier);
        }


        public async Task<IActionResult> SupplierEdit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }
            var supplier = await s.SupplierDetails(id);
            return View(supplier);
        }
        [HttpPost]
        public IActionResult SupplierEdit (Supplier supplier)
		{
			if(supplier.PhotoPath == null) 
			{ 
				string? PhotoPath = _context.Suppliers.FirstOrDefault(s => s.SupplierID == supplier.SupplierID).PhotoPath;
				supplier.PhotoPath = PhotoPath;
			}
			bool answer = Cls_Supplier.SupplierUpdate(supplier);

			TempData["Message"] = answer ? "Güncellendi" : "HATA";

			return RedirectToAction(nameof(SupplierEdit));
		}


		public async Task<IActionResult> SupplierDelete(int? id)
		{
			if (id == null || _context.Suppliers == null)
				return NotFound();

			var supplier = await _context.Suppliers.FirstOrDefaultAsync(c => c.SupplierID == id);

			if (supplier == null)
				return NotFound();

			return View(supplier);

		}

		[HttpPost, ActionName("SupplierDelete")]
		public async Task<IActionResult> SupplierDeleteConfirmed(int id)
		{
			bool answer = await Cls_Supplier.SupplierDelete(id);

			if (answer)
			{
				TempData["Message"] = "Silindi";
				return RedirectToAction("SupplierIndex");
			}
			else
			{
				TempData["Message"] = "Hata";
				return RedirectToAction("SupplierDelete");
			}

		}

		public async Task<IActionResult> StatusIndex()
        {
            List<Status> statuses = await st.StatusSelect();
            return View(statuses);
        }


		public IActionResult StatusCreate()
		{
			return View();
		}


		[HttpPost]
		public IActionResult StatusCreate(Status status)
		{
			bool answer = Cls_Status.StatusInsert(status);

			TempData["Message"] = answer ? status.StatusName + " Durumu Eklendi." : "HATA " + status.StatusName + " Durumu Eklenemedi.";

			return RedirectToAction(nameof(StatusCreate));

		}

		public async Task<IActionResult> StatusEdit(int? id)
		{
			if (id == null || _context.Statuses == null)
			{
				return NotFound();
			}
			var statuses = await st.StatusDetails(id);
			return View(statuses);
        }
        [HttpPost]
        public IActionResult StatusEdit(Status status)
        {
            
            bool answer = Cls_Status.StatusUpdate(status);

            TempData["Message"] = answer ? "Güncellendi" : "HATA";

            return RedirectToAction(nameof(StatusEdit));
        }
        public async Task<IActionResult> StatusDelete(int? id)
        {
            if (id == null || _context.Statuses == null)
                return NotFound();

            var status = await _context.Statuses.FirstOrDefaultAsync(st => st.StatusID == id);

            if (status == null)
                return NotFound();

            return View(status);

        }

        [HttpPost, ActionName("StatusDelete")]
        public async Task<IActionResult> StatusDeleteConfirmed(int id)
        {
            bool answer = await Cls_Status.StatusDelete(id);

            if (answer)
            {
                TempData["Message"] = "Silindi";
                return RedirectToAction("StatusIndex");
            }
            else
            {
                TempData["Message"] = "Hata";
                return RedirectToAction("StatusDelete");
            }

        }

		public async Task<IActionResult> ProductIndex()
		{
			List<Product> products = await p.ProductSelect();
			return View(products);
		}


		public async Task<IActionResult> ProductCreate()
		{
			List<Category> categories = await c.CategorySelect();
			ViewData["categoryList"] = categories.Select(c=> new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });


			List<Supplier> suppliers = await s.SupplierSelect();
			ViewData["supplierList"] = suppliers.Select(s => new SelectListItem { Text = s.BrandName, Value = s.SupplierID.ToString() });

			List<Status> statuses = await st.StatusSelect();
			ViewData["statusList"] = statuses.Select(st => new SelectListItem { Text = st.StatusName, Value = st.StatusID.ToString() });

			return View();
		}

		[HttpPost]
		public IActionResult ProductCreate(Product product)
		{
			bool answer = Cls_Product.ProductInsert(product);

			TempData["Message"] = answer ? product.ProductName + " Ürünü Eklendi." : "HATA " + product.ProductName + " Ürünü Eklenemedi.";

			return RedirectToAction(nameof(ProductCreate));

		}


		public async Task<IActionResult> ProductEdit(int? id)

		{

			CategoryFill();
			SupplierFill();

			StatusFill();

			if (id == null || _context.Products == null)

			{

				return NotFound();

			}

			var product = await p.ProductDetails(id);

			return View(product);

		}

		[HttpPost]

		public IActionResult ProductEdit(Product product)
		{

			//veritabanından kaydını getirdim

			Product prd = _context.Products.FirstOrDefault(s => s.ProductID == product.ProductID);

			//formdan gelmeyen , bazı kolonları null yerine , eski bilgilerini bastım

			product.AddDate = prd.AddDate;

			product.HighLighted = prd.HighLighted;

			product.TopSeller = prd.TopSeller;
			if (product.PhotoPath == null)
			{

				string? PhotoPath = _context.Products.FirstOrDefault(s => s.ProductID == product.ProductID).PhotoPath;

				product.PhotoPath = PhotoPath;

			}

			bool answer = Cls_Product.ProductUpdate(product);

			if (answer == true)

			{

				TempData["Message"] = "Güncellendi";

				return RedirectToAction("ProductIndex");

			}

			else

			{

				TempData["Message"] = "HATA";

				return RedirectToAction(nameof(ProductEdit));

			}

		}

		public async Task<IActionResult> ProductDetails(int? id)

		{
			var product = await _context.Products.FirstOrDefaultAsync(s => s.ProductID == id);
			ViewBag.productname = product?.ProductName;

			return View(product);

		}


        [HttpGet]
        public async Task<IActionResult> ProductDelete(int? id)
        {
            if (id == null || _context.Products == null)
                return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(c => c.ProductID == id);

            if (product == null)
                return NotFound();

            return View(product);

        }


        [HttpPost, ActionName("ProductDelete")]
        public async Task<IActionResult> ProductDeleteConfirmed(int id)
        {
            bool answer = await Cls_Product.ProductDelete(id);

            if (answer)
            {
                TempData["Message"] = "Silindi";
                return RedirectToAction("ProductIndex");
            }
            else
            {
                TempData["Message"] = "Hata";
                return RedirectToAction("ProductDelete");
            }

        }
        async void SupplierFill()

		{

			List<Supplier> suppliers = await s.SupplierSelect();

			ViewData["supplierList"] = suppliers.Select(s => new SelectListItem { Text = s.BrandName, Value = s.SupplierID.ToString() });

		}

		async void StatusFill()

		{

			List<Status> statuses = await st.StatusSelect();

			ViewData["statusList"] = statuses.Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusID.ToString() });

		}

	}
}

