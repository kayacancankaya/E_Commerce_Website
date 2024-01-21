using Microsoft.EntityFrameworkCore;

namespace IakademiWebProject.Models
{
    public class Cls_Supplier
    {

        iakademi38Context _context = new();
        public async Task<List<Supplier>> SupplierSelect()
        {
            List<Supplier> suppliers = await _context.Suppliers.ToListAsync();

            return suppliers;
        }

		public static bool SupplierInsert(Supplier supplier)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					context.Add(supplier);
					context.SaveChanges();
				}

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");

				return false;
			}
		}

        public async Task<Supplier> SupplierDetails(int? id)
        {
            Supplier? supplier = await _context.Suppliers.FirstOrDefaultAsync(s=>s.SupplierID == id);
            return supplier;
        }

        public static bool SupplierUpdate(Supplier supplier)
        {
            try
            {
                using (iakademi38Context context = new iakademi38Context())
                {
                    context.Update(supplier);
                    context.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                return false;
            }

        }

		public static async Task<bool> SupplierDelete(int id)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					Supplier supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == id);
					supplier.Active = false;

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
	}
}
