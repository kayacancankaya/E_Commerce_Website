using IakademiWebProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace IakademiWebProject.ViewComponents
{
	public class Address : ViewComponent
	{
		iakademi38Context context = new iakademi38Context();

		public string Invoke()
		{
			string address = context.Settings.FirstOrDefault(s => s.SettingID == 1).Address;
			return $"{address}";
		}
	}
}
