using XAct;

namespace IakademiWebProject.Models
{
    public class Cls_Order
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string MyCart { get; set; }

        public decimal UnitPrice { get; set; }
        public string? ProductName { get; set; }

        public int Kdv { get; set; }

        public string? PhotoPath { get; set; }

        public string? TcKimlik_VeriNo { get; set; }

        iakademi38Context _context = new();

        public string AddToMyCart(string id,string cookie)
        {
            try
            {
                bool exists = false;
                
                if (cookie == "") 
                {
                    MyCart = id + "=1";
                    return MyCart;
                }
                else
                {
                    string[] MyCartArray = cookie.Split('&');
                    string updatedExistingID = string.Empty;
                    bool firstItem = true;
                    foreach (string item in MyCartArray) 
                    { 
                        string[] existingID = item.Split('=');

                        if (existingID[0] == id)
                        {
                            exists = true;
                            existingID[1] = (Convert.ToInt32(existingID[1]) + 1).ToString();
                        }
                        //eğer ilk ürün ise & ekleme
                        if(firstItem)
                            updatedExistingID = updatedExistingID + string.Join("=", existingID);
                        else
                            updatedExistingID =  updatedExistingID + "&" + string.Join("=", existingID);

                        firstItem = false;
                    }
                    if (!exists)
                    { 
                       MyCart = cookie + "&" + id.ToString() + "=1";
                        return MyCart;
                    }

                    else 
                    {
                        MyCart = updatedExistingID;
                        return MyCart;
                    }
                }
            }
            catch
            {
                return "HATA";
            }
        }

        public string DeleteFromMyCart(string id, string cookie) 
        {
            try
            {

                if (string.IsNullOrEmpty(cookie))
                {
                    return "BOS COOKIE";
                }
                if (string.IsNullOrEmpty(cookie))
                {
                    return "BOS ID";
                }
                
                string[] MyCartArray = cookie.Split('&');
                List<string> updatedExistingIDs = new List<string>();
                foreach (string item in MyCartArray)
                {
                    string[] existingID = item.Split('=');
                    
                    if (existingID[0] != id)
                    {
                        updatedExistingIDs.Add(string.Join("=", existingID));
                    }
                }
                MyCart = string.Empty;
                int counter = 0;

                if(updatedExistingIDs.Count == 1)
                    MyCart = updatedExistingIDs.FirstOrDefault();
                else
                { 
                    foreach (string item in updatedExistingIDs) 
                    { 
                        if (counter != updatedExistingIDs.Count) 
                        { 
                            MyCart = MyCart + item + "&";
                        }
                    }
                }
                return MyCart;
            }
            catch
            {
                return "HATA";
            }
        }

        public List<Cls_Order> SelectMyCart(string cookies)
        {
            try
            {
                List<Cls_Order> list = new();
                
                if(string.IsNullOrEmpty(cookies)) { return list; }

                string[] MyCartArray = cookies.Split("&");
                if (MyCartArray[0] == "")
                    return list;

                for(int i = 0; i < MyCartArray.Length; i++) 
                {
                    string[] MyCartArrayLoop = MyCartArray[i].Split("=");
                    int MyCartID = Convert.ToInt32(MyCartArrayLoop[0]);
                    Product products = _context.Products.FirstOrDefault(p=>p.ProductID == MyCartID);
                    
                    Cls_Order order = new ();
                    order.ProductID = products.ProductID;
                    order.Quantity = Convert.ToInt32(MyCartArrayLoop[1]);
                    order.UnitPrice = products.UnitPrice;
                    order.ProductName = products.ProductName;
                    order.PhotoPath = products.PhotoPath;
                    order.Kdv = products.Kdv;
                    list.Add(order);
                }

                return list;
            }
            catch 
            {
                return null;
            }
        }


        public void EfaturaCreate()
        {

        }

        public string OrderCreate(string Email, string cookies)
        {
            List<Cls_Order> sipList = SelectMyCart(cookies);
            string OrderGroupGUID = DateTime.Now.ToString().Replace(":", "").Replace(" ", "").Replace(".", "");
            DateTime OrderDate = DateTime.Now; ;

            foreach (var item in sipList)
            {
                Order order = new Order();
                order.OrderDate = OrderDate;
                order.OrderGroupGUID = OrderGroupGUID;
                order.UserID = _context.Users.FirstOrDefault(u => u.Email == Email).UserID;
                order.ProductID = item.ProductID;
                order.Quantity = item.Quantity;
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            return OrderGroupGUID;
        }

		public List<vw_MyOrders> SelectMyOrders(string Email)
		{
			int UserID = _context.Users.FirstOrDefault(u => u.Email == Email).UserID;

			List<vw_MyOrders> myOrders = _context.vw_MyOrders.Where(o => o.UserID == UserID).ToList();

			return myOrders;
		}
	}

}
