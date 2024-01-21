using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using XAct;

namespace IakademiWebProject.Models
{
    public class Cls_Product
    {
        iakademi38Context _context = new();
        public async Task<List<Product>> ProductSelect()
        {
            List<Product> products = await _context.Products.ToListAsync();

            return products;
        }
        public static bool ProductInsert(Product product)
        {
            try
            {
                //metod static oldugu icin , context burada tanımlamak zorundayım
                using (iakademi38Context context = new iakademi38Context())
                {
                    product.AddDate = DateTime.Now;
                    context.Add(product);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
		public static bool ProductUpdate(Product product)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					context.Update(product);
					context.SaveChanges();
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<Product> ProductDetails(int? id)
		{
			Product? product = await _context.Products.FindAsync(id);
			return product;
		}


        public static async Task<bool> ProductDelete(int id)
        {
            try
            {
                using (iakademi38Context context = new iakademi38Context())
                {
                    Product product = await context.Products.FirstOrDefaultAsync(c => c.ProductID == id);
                    product.Active = false;

                    await context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                return false;
            }

        }

        public Product ProductDetails(string mainPageName)
		{
			Product product = _context.Products.FirstOrDefault(p => p.StatusID == 6);
			return product;
		}

        public List<Product> ProductSelect(string mainPageName, int mainPageCount,
                                            string subPageName, int pageNumber,int subPageCount )
        {
            List<Product> products = new();

            if (mainPageName == "Slider")
            {
                products = _context.Products.Where(p => p.StatusID == 1 && p.Active == true).ToList();

            }
            else if (mainPageName == "New")
            {
                if (subPageName == "")
                    products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.AddDate).Take(mainPageCount).ToList();
                else
                {
                    if (pageNumber == 0)
                    {
                        products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.AddDate).Take(subPageCount).ToList();
                    }
                    else
                    {
                        products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.AddDate).Skip(pageNumber * subPageCount).Take(subPageCount).ToList();
                    }
                }
            }
            else if (mainPageName == "Special")
            {
                if(subPageName == "Ana")
                { 
                    products = _context.Products.Where(p => p.StatusID == 2 && p.Active == true).Take(mainPageCount).ToList();
                }
                else {
                    products = _context.Products.Where(p => p.StatusID == 2 && p.Active == true).Skip(pageNumber*subPageCount).Take(subPageCount).ToList();
                }
			}
            else if (mainPageName == "Discounted")
            {
                if (subPageName == "Ana")
                    products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.Discount).Take(mainPageCount).ToList();
                else
                {
                    if (pageNumber == 0)
                    {
                        products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.Discount).Take(subPageCount).ToList();
                    }
                    else
                    {
                        products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.Discount).Skip(pageNumber * subPageCount).Take(subPageCount).ToList();
                    }
                }
              
			}
            else if (mainPageName == "HighLighted")
            {

                if (subPageName == "Ana")
                    products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.HighLighted).Take(mainPageCount).ToList();
                else
                {
                    if (pageNumber == 0)
                    {
                        products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.HighLighted).Take(mainPageCount).ToList();
                    }
                    else
                    {
                        products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.HighLighted).Skip(pageNumber * subPageCount).Take(subPageCount).ToList();
                    }
                }

                
			}
            else if (mainPageName == "TopSeller")
            {
                products = _context.Products.Where(p => p.Active == true).OrderByDescending(p => p.TopSeller).Take(mainPageCount).ToList();
			}
            else if (mainPageName == "Star")
            {
				products = _context.Products.Where(p => p.StatusID == 3 && p.Active == true).OrderByDescending(p => p.ProductName).Take(mainPageCount).ToList();
			}
            else
                products = _context.Products.Where(p => p.Active == true).ToList();

            return products;
		}

        public List<Product> ProductDetailedSearch(string query)
        {
            string ConnectionString = "Server=localhost;Database=IakademiWebProjectDb;Trusted_Connection=True;TrustServerCertificate=True;";

            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);

            SqlDataReader sdr = command.ExecuteReader();

            List<Product> model = new();

            sdr.Read();
            if (sdr.FieldCount > 0 &&
                sdr != null)
            {
                while (sdr.Read())
                {
                    Product product = new Product
                    {
                        ProductID = sdr.GetInt32(0),
                        ProductName = sdr.GetString(1),
                        UnitPrice = sdr.GetDecimal(2),
                        CategoryID = sdr.GetInt32(3),
                        SupplierID = sdr.GetInt32(4),
                        Stock = sdr.GetInt32(5),
                        Discount = sdr.GetInt32(6),
                        StatusID = sdr.GetInt32(7),
                        AddDate = sdr.GetDateTime(8),
                        Keywords = sdr.GetString(9),
                        Kdv = sdr.GetInt32(10),
                        HighLighted = sdr.GetInt32(11),
                        TopSeller = sdr.GetInt32(12),
                        Related = sdr.GetInt32(13),
                        Notes = sdr.GetString(14),
                        PhotoPath = sdr.GetString(15),
                        Active = sdr.GetBoolean(16)
                    };

                    model.Add(product);
                }
            }
            return(model);
        }
	}
}
