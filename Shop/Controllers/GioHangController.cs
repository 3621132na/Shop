using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class GioHangController : Controller
    {
        ShopDataDataContext data = new ShopDataDataContext();
        public PartialViewResult Index()
        {
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView(lstGiohang);
        }
        public PartialViewResult Gia()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGiohang(int iMaSp, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(n => n.iMaSp == iMaSp);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMaSp);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            return iTongTien;
        }
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
                return RedirectToAction("Index", "Home");
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {

            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaSp == iMaSP);
            if (sanpham != null)
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            return RedirectToAction("Giohang");
        }
        public ActionResult XoaGiohang(int iMaSP)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaSp == iMaSP);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMaSp == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
                return RedirectToAction("Index", "Home");
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult DatHang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
                return RedirectToAction("Dangnhap", "Nguoidung");
            if (Session["Giohang"] == null)
                return RedirectToAction("Index", "Home");
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
                return RedirectToAction("Index", "Home");
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            var ghichu = collection["Ghichu"];
            ddh.Ghichu = ghichu;
            ddh.Tinhtranggiaohang = "dang giao";
            var tongtien=(decimal)TongTien();
            ddh.TongGia = tongtien;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSP = item.iMaSp;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDongia;
                data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang", new {maDonHang  = ddh.MaDonHang });
        }
        public ActionResult Xacnhandonhang(int maDonHang)
        {
            var chiTietDonHang = new List<ChiTietDonHangViewModel>();

            var chiTietDonHangGoc = data.CHITIETDONTHANGs.Where(ct => ct.MaDonHang == maDonHang).ToList();
            foreach (var item in chiTietDonHangGoc)
            {
                var tenSanPham = data.SANPHAMs.FirstOrDefault(sp => sp.MaSP == item.MaSP)?.TenSP;
                if (tenSanPham != null)
                {
                    var chiTietDonHangItem = new ChiTietDonHangViewModel
                    {
                        TenSP = tenSanPham,
                        Soluong = item.Soluong,
                        Dongia = item.Dongia,
                        ThanhTien = (decimal)(item.Soluong * item.Dongia)
                    };
                    chiTietDonHang.Add(chiTietDonHangItem);
                }
            }
            ViewBag.ChiTietDonHang = chiTietDonHang;
            var tong = data.CHITIETDONTHANGs.Where(ct => ct.MaDonHang == maDonHang).Sum(ct => ct.Soluong * ct.Dongia);
            ViewBag.Tongtien = tong;
            return View();          
        }
    }
}