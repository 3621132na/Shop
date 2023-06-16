using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        ShopDataDataContext data = new ShopDataDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Search(String search)
        {
            var sanpham=from sp in data.SANPHAMs select sp;
            if(!String.IsNullOrEmpty(search))
                sanpham=sanpham.Where(s=>s.TenSP.Contains(search));
            return View(sanpham);
        }
        public ActionResult ThongTinCuaHang()
        {
            return View();
        }
        public ActionResult Blog()  
        {
            return View();
        }
        public PartialViewResult DanhMuc()
        {
            var danhmuc=(from dm in data.DANHMUCs select dm).ToList();
            return PartialView(danhmuc);
        }
        public PartialViewResult Loai(int danhmuc)
        {
            var loai=(from l in data.LOAIs where l.MaDM==danhmuc select l).ToList();
            return PartialView(loai);
        }
        public PartialViewResult SanPhamBanChay()
        {
            var sanpham=(from sp in data.SANPHAMs select sp).ToList();
            return PartialView(sanpham);
        }
        public PartialViewResult NhaCungCap()
        {
            var nhacungcap=(from ncc in data.NHACUNGCAPs select ncc).ToList();
            return PartialView(nhacungcap);
        }
    }
}