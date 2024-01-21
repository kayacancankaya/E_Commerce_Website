﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IakademiWebProject.Models
{
    public class Pharmacy
    {
        public DateTime Tarih { get; set; }
        public string? LokasyonY { get; set; }
        public string? LokasyonX { get; set; }
        public string? Adi { get; set; }
        public string? Telefon { get; set; }
        public string? Adres { get; set; }

    }
}
