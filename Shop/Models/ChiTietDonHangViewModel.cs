using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class ChiTietDonHangViewModel
    {
        public string TenSP { get; set; }
        public int? Soluong { get; set; }
        public decimal? Dongia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}