using IakademiWebProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace IakademiWebProject.ViewComponents
{
	public class Phone : ViewComponent
	{
		iakademi38Context context = new iakademi38Context();

		public string Invoke()
		{
			string phone = context.Settings.FirstOrDefault(s => s.SettingID == 1).Telephone;
			return $"{phone}";
		}
	}
}
