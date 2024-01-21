using Microsoft.EntityFrameworkCore;

namespace IakademiWebProject.Models
{
    public class Cls_Category
	{
		iakademi38Context _context = new();
		public async Task<List<Category>> CategorySelect()
		{
			List<Category> categories = await _context.Categories.ToListAsync();

			return categories;
		}

		public static bool CategoryInsert(Category category)
		{
			try
			{
				//method static olduğu için context instanceı yeniden yaratılıyor.
				using (iakademi38Context context = new iakademi38Context())
				{
					context.Add(category);
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

		public async Task<Category> CategoryDetails(int? id)
		{
			Category? categories = await _context.Categories.FindAsync(id);
			return categories;
		}

		public static bool CategoryUpdate(Category category)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					context.Update(category);
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
		public static async Task<bool> CategoryDelete(int id)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
                    Category category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);
					category.Active = false;

					List<Category> categoryList = await context.Categories.Where(c => c.ParentID == id).ToListAsync();

					foreach(var item in categoryList)
					{
						item.Active = false;
					}
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
