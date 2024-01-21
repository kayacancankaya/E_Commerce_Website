using Microsoft.AspNetCore.Mvc;
using IakademiWebProject.Models;
using PagedList.Core;
using XAct;
using Microsoft.Data.SqlClient;

namespace IakademiWebProject.Controllers
{
    public class HomeController : Controller
    {
        MainPageModel mpm = new();
        Cls_Product p = new ();
        Cls_Order o = new ();
        Cls_User u = new ();
        iakademi38Context _context = new();
        int mainPageCount = 0;
        int subPageCount = 0;
        public HomeController()
        {
           this.mainPageCount = _context.Settings.FirstOrDefault(i=>i.SettingID == 1).MainPageCount;
           this.subPageCount = _context.Settings.FirstOrDefault(i=>i.SettingID == 1).SubPageCount;
        }
        public IActionResult Index()
        {
            mpm.SliderProducts = p.ProductSelect("Slider", mainPageCount,"Ana",0, subPageCount);
            mpm.Productoftheday = p.ProductDetails("ProductOfTheDay");
            mpm.NewProducts = p.ProductSelect("New", mainPageCount, "Ana", 0, subPageCount);
            mpm.SpecialProducts = p.ProductSelect("Special", mainPageCount,"Ana",0, subPageCount);
            mpm.DiscountedProducts = p.ProductSelect("Discount", mainPageCount,"Ana",0, subPageCount);
            mpm.HighLightedProducts = p.ProductSelect("Highlighted", mainPageCount,"Ana",0, subPageCount);
            mpm.TopSellerProducts = p.ProductSelect("TopSeller", mainPageCount,"Ana",0, subPageCount);
            mpm.StarProducts = p.ProductSelect("Star", mainPageCount,"Ana",0, subPageCount);
            return View(mpm);
        }
        public IActionResult Details(int id)
        {
            return View();
        }
        public IActionResult NewProducts()
        {
            mpm.NewProducts = p.ProductSelect("New", mainPageCount, "New", 0, subPageCount);
           return View(mpm);
        } 
        public IActionResult SpecialProducts()
        {
            mpm.SpecialProducts = p.ProductSelect("Special", mainPageCount, "Special", 0, subPageCount);
           return View(mpm);
        }
        public IActionResult DiscountedProducts()
        {
            mpm.DiscountedProducts = p.ProductSelect("Discounted", mainPageCount, "Discounted", 0, subPageCount);
            return View(mpm);
        }
        public IActionResult HighlightedProducts()
        {
            mpm.HighLightedProducts = p.ProductSelect("Highlighted", mainPageCount, "Highlighted", 0, subPageCount);
            return View(mpm);
        }
        public IActionResult TopsellerProducts(int page = 1, int pageSize = 4)
        {
            PagedList<Product> model = new PagedList<Product>(_context.Products.OrderByDescending(p => p.TopSeller), page, pageSize);

            return View("TopsellerProducts", model);
        }

        public IActionResult CategoryPage(int categoryID)
        {
            List<Product> model = new List<Product>();
            var products = _context.Products.Where(i => i.CategoryID == categoryID).ToList();

            foreach ( var product in products)  
                model.Add(product); 

            return View(model);
		}
		public IActionResult SupplierPage(int supplierID)
		{
			List<Product> model = new List<Product>();
			var products = _context.Products.Where(i => i.SupplierID == supplierID).ToList();

			foreach (var product in products)
				model.Add(product);

			return View(model);
		}

		//.net core http install
		public IActionResult CartProcess(int id)
        {
            
            //Cls_Product.Highlighted_Increase(id);
            o.ProductID = id;
            o.Quantity = 1;

            var cookieOptions = new CookieOptions();

            //read
            //10=1&21=3
            string url = Request.Headers["Referer"].ToString();
            var cookie = Request.Cookies["sepetim"];//tarayıcıdan halihazırdaki çerezleri getir
            if (cookie == null)
            {
                cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(7);
                cookieOptions.Path = "/";
                o.MyCart = "";
                o.MyCart = o.AddToMyCart(id.ToString(),"");

                if(o.MyCart == "HATA")
                { TempData["Message"] = "Ürün Sepete Eklenirken Hata İle Karşılaşıldı.";  return Redirect(url); }

                Response.Cookies.Append("sepetim", o.MyCart, cookieOptions); // tarayıcıya gönderdik
                //HttpContext.Session.SetString("Message", "Ürününüz Sepete Eklendi.");
                TempData["Message"] = "Ürününüz Sepete Eklendi.";

            }
            else
            {
                o.MyCart = o.AddToMyCart(id.ToString(), cookie);

                if (o.MyCart == "HATA")
                { TempData["Message"] = "Ürün Sepete Eklenirken Hata İle Karşılaşıldı."; return Redirect(url); }
                Response.Cookies.Append("sepetim", o.MyCart, cookieOptions); // tarayıcıya gönderdik

                //HttpContext.Session.SetString("Message", "Ürününüz Sepete Eklendi.");
                TempData["Message"] = "Ürününüz Sepete Eklendi.";

            }

            
            return Redirect(url);
        }

        // sağ üst köşeden tıklayınca ürünü sile basınca 
        public IActionResult Cart()
        {
            List<Cls_Order> sepet;
            //string? scid = HttpContext.Request.Query["scid"];//ürün silerken sil butonu ile productid göndereceğim
            var cookieOptions = new CookieOptions();
          
            
                string? scid = HttpContext.Request.Query["scid"];
                string? cookies = Request.Cookies["sepetim"];
                
                if(scid != null && cookies != null)
                { 
                o.MyCart = o.DeleteFromMyCart(scid, cookies);
                if (o.MyCart == "HATA")
                {
                    TempData["Message"] = "Sepetim Sayfası Oluşturulurken Hata İle Karşılaşıldı";
                    cookieOptions.Expires = DateTime.Now.AddDays(-1);
                    return View();
                }
                if(o.MyCart == "BOS COOKIE")
                { 
                    o.MyCart = "";
                    List<Cls_Order> bosSepet = new List<Cls_Order>();
                    ViewBag.Sepetim = bosSepet;
                    ViewBag.sepet_tablo_detay = bosSepet;
                    return View();
                }
                if(o.MyCart == "BOS ID")
                {
                    cookieOptions = new CookieOptions();
                    o.MyCart = Request.Cookies["sepetim"];
                    sepet = o.SelectMyCart(cookies);
                    ViewBag.Sepetim = sepet;
                    ViewBag.sepet_tablo_detay = sepet;
                    return View();
                }
                Response.Cookies.Append("sepetim", o.MyCart, cookieOptions);
                cookieOptions.Expires = DateTime.Now.AddDays(7);

                TempData["Message"] = "Ürün Sepetten Silindi";

                sepet = o.SelectMyCart(cookies);
                ViewBag.Sepetim = sepet;
                ViewBag.sepet_tablo_detay = sepet;
                return View();
            }
                if(cookies != null)
                { 
                    Response.Cookies.Append("sepetim", cookies, cookieOptions);
                    cookieOptions.Expires = DateTime.Now.AddDays(7);
                    sepet = o.SelectMyCart(cookies);
                    ViewBag.Sepetim = sepet;
                    ViewBag.sepet_tablo_detay = sepet;
                }
                else
                {
                    o.MyCart = "";
                    List<Cls_Order> bosSepet = new List<Cls_Order>();
                    ViewBag.Sepetim = bosSepet;
                    ViewBag.sepet_tablo_detay = bosSepet;
                    return View();
                }


            return View();
        }

        public IActionResult Order()
        { 
            if(HttpContext.Session.GetString("Email") != null)
            {
                User? usr = Cls_User.SelectMemberInfo(HttpContext.Session.GetString("Email"));
                return View(usr);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        [HttpPost]
        public IActionResult Order(IFormCollection frm)
        {
            string txt_individual = Request.Form["txt_individual"];
            string txt_corporate = Request.Form["txt_corporate"];

            if (txt_individual != null)
            {
                //bireysel fatura
                //digital planet
                //WebServiceController.tckimlik_vergi_no = txt_individual;
                o.TcKimlik_VeriNo = txt_individual;
                o.EfaturaCreate();
            }
            else
            {
                //kurumsal fatura
                //WebServiceController.tckimlik_vergi_no = txt_corporate;
                o.TcKimlik_VeriNo = txt_corporate;
                o.EfaturaCreate();
            }

            string kredikartno = Request.Form["kredikartno"];
            string kredikartay = frm["kredikartay"];
            string kredikartyil = frm["kredikartyil"];
            string kredikartcvs = frm["kredikartcvs"];

            return RedirectToAction("backref");

            //buradan sonraki kodlar , payu , iyzico

            //payu dan gelen örnek kodlar

            /*  AŞAGIDAKİ KODLAR GERÇEK HAYATTA AÇILALAK
            NameValueCollection data = new NameValueCollection();
            string url = "https://www.sedattefci.com/backref";
 
            data.Add("BACK_REF", url);
            data.Add("CC_CVV", kredikartcvs);
            data.Add("CC_NUMBER", kredikartno);
            data.Add("EXP_MONTH", kredikartay);
            data.Add("EXP_YEAR", "20" + kredikartyil);
 
            var deger = "";
 
            foreach (var item in data)
            {
                var value = item as string;
                var byteCount = Encoding.UTF8.GetByteCount(data.Get(value));
                deger += byteCount + data.Get(value);
            }
 
            var signatureKey = "size verilen SECRET_KEY buraya yazılacak";
 
            var hash = HashWithSignature(deger, signatureKey);
 
            data.Add("ORDER_HASH", hash);
 
            var x = POSTFormPAYU("https://secure.payu.com.tr/order/....", data);
 
            //sanal kart
            if (x.Contains("<STATUS>SUCCESS</STATUS>") && x.Contains("<RETURN_CODE>3DS_ENROLLED</RETURN_CODE>"))
            {
                //sanal kart (debit kart) ile alış veriş yaptı , bankadan onay aldı
            }
            else
            {
                //gerçek kart ile alış veriş yaptı , bankadan onay aldı
            }
            */
        }

        public IActionResult backref()
        {
            ConfirmOrder();
            return RedirectToAction("ConfirmPage");
        }

        public static string OrderGroupGUID = "";

        public IActionResult ConfirmOrder()
        {
            //sipariş tablosuna kaydet
            //sepetim cookie sinden sepeti temizleyecegiz
            //e-fatura olustur metodunu cagır
            var cookieOptions = new CookieOptions();
            var cookie = Request.Cookies["sepetim"];
            if (cookie != null)
            {
                o.MyCart = cookie;
                OrderGroupGUID = o.OrderCreate(HttpContext.Session.GetString("Email").ToString(),cookie);

                cookieOptions.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Delete("sepetim"); //tarayıcıdan sepeti sil
                                                    //    cls_User.Send_Sms(OrderGroupGUID);
                                                    //   cls_User.Send_Email(OrderGroupGUID);
            }
            return RedirectToAction("ConfirmPage");
        }

        public IActionResult ConfirmPage()
        {
            ViewBag.OrderGroupGUID = OrderGroupGUID;
            return View();
        }

		public IActionResult MyOrders()
		{
			if (HttpContext.Session.GetString("Email") != null)
			{
				List<vw_MyOrders> orders = o.SelectMyOrders(HttpContext.Session.GetString("Email").ToString());
				return View(orders);
			}
			else
			{
				return RedirectToAction("Login");
			}
		}

		public IActionResult Login()
        { return View(); }

        [HttpPost]
        public IActionResult Login(User user)
        {
            string answer = Cls_User.MemberControl(user);

            if (answer == "error")
            {
                HttpContext.Session.SetString("Mesaj", "Email/Şifre yanlış girildi");
                TempData["Message"] = "Email/Şifre yanlış girildi";
                return View();
            }
            else if (answer == "admin")
            {
                HttpContext.Session.SetString("Email", answer);
                HttpContext.Session.SetString("Admin", answer);
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Index");
            }
        }

        public IActionResult Register()
        { 
            return View(); 
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if(string.IsNullOrEmpty(user.Email))
            {
                TempData["Message"] = "Email Giriniz.";
                return Register(); 
            }

            if (Cls_User.LoginEmailControl(user.Email) == false)
            {
                bool answer = Cls_User.AddUser(user);

                if (answer)
                {
                    TempData["Message"] = "Kaydedildi.";
                    return RedirectToAction("Login");
                }
                TempData["Message"] = "Hata.Tekrar deneyiniz.";
            }
            else
            {
                TempData["Message"] = "Bu Email Zaten mevcut.Başka Deneyiniz.";
            }
            return View();
        }

        public IActionResult Logout()
        {
			HttpContext.Session.Remove("Email");
			HttpContext.Session.Remove("Admin");
            return RedirectToAction("Index");
		}
        public List<Product> ProductSelect(string mainPageName)
        {
            List<Product> sliderProducts = _context.Products.Where(p=>p.Active == true).ToList();

            return sliderProducts;
        }

        public IActionResult DetailedSearch() 
        { 
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            return View();
        }

        public IActionResult DProducts(int CategoryID, string[] SupplierID, string price, string IsInStock )
        {
            price = price.Replace(" ", "");
            string[] PriceArray = price.Split('-');
            string startprice = PriceArray[0].Replace("$","");
            string endprice = PriceArray[1].Replace("$", "");

            string sign = ">";
            if(IsInStock == "0")
                sign = ">=";

            int count = 0;

            string suppliervalue = "";

            if(SupplierID.Count() == 0)
                suppliervalue = string.Empty;
            else if (SupplierID.Count() == 1)
                suppliervalue = " and SupplierID = " + SupplierID[0];
            else
            {
                for(int i = 0;i < SupplierID.Count(); i++)
                {
                    suppliervalue += "SupplierID = " + SupplierID[i] + " or ";
                }
                suppliervalue = suppliervalue.Left(suppliervalue.Length - 3);
                suppliervalue = "and (" + suppliervalue + ")";
            }

            string query = $"select * from products where CategoryID = {CategoryID} {suppliervalue} and (UnitPrice > {startprice} and UnitPrice < {endprice}) and Stock{sign}{IsInStock}";

            ViewBag.Products = p.ProductDetailedSearch(query);

            return View();
        }

        public PartialViewResult _PartialNewProducts(string pageno)
        {
            mpm.NewProducts = p.ProductSelect("New", mainPageCount, "New", Convert.ToInt32(pageno), subPageCount);
            return PartialView(mpm);
        }
        public PartialViewResult _PartialSpecialProducts(string pageno)
        {
            mpm.SpecialProducts = p.ProductSelect("Special", mainPageCount, "Special", Convert.ToInt32(pageno), subPageCount);
            return PartialView(mpm);
        }
        public PartialViewResult _PartialDiscountedProducts(string pageno)
        {
            mpm.DiscountedProducts = p.ProductSelect("Discounted", mainPageCount, "Discounted", Convert.ToInt32(pageno), subPageCount);
            return PartialView(mpm);
        }
        public PartialViewResult _PartialHighlightedProducts(string pageno)
        {
            mpm.HighLightedProducts = p.ProductSelect("Highlighted", mainPageCount, "Highlighted", Convert.ToInt32(pageno), subPageCount);
            return PartialView(mpm);
        }

    }
}
