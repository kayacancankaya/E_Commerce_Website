using Microsoft.EntityFrameworkCore;

namespace IakademiWebProject.Models
{
    public class Cls_Status
    {

        iakademi38Context _context = new();
        public async Task<List<Status>> StatusSelect()
        {
            List<Status> statuses = await _context.Statuses.ToListAsync();

            return statuses;
        }
		public static bool StatusInsert(Status status)
		{
			try
			{
				using (iakademi38Context context = new iakademi38Context())
				{
					context.Add(status);
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

		public async Task<Status> StatusDetails(int? id)
		{
			Status? status = await _context.Statuses.FirstOrDefaultAsync(s => s.StatusID == id);
			return status;
		}

        public static bool StatusUpdate(Status status)
        {
            try
            {
                using (iakademi38Context context = new iakademi38Context())
                {
                    context.Update(status);
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

        public static async Task<bool> StatusDelete(int id)
        {
            try
            {
                using (iakademi38Context context = new iakademi38Context())
                {
                    Status status = await context.Statuses.FirstOrDefaultAsync(s => s.StatusID == id);
                    status.Active = false;

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
