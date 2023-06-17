using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class NguoiDungController : Controller
    {
        ShopDataDataContext data = new ShopDataDataContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG kh)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            var nhaplaimk = collection["nhaplaiMK"];
            var diachi = collection["Diachi"];
            var email = collection["Email"];
            var dienthoai = collection["Dienthoai"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            if (String.IsNullOrEmpty(hoten))
                ViewData["Loi1"] = "Họ tên khách hàng không được bỏ trống";
            else if (String.IsNullOrEmpty(tendn))
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            else if (String.IsNullOrEmpty(matkhau))
                ViewData["Loi3"] = "Phải nhập mật khẩu";
            else if (String.IsNullOrEmpty(nhaplaimk))
                ViewData["Loi4"] = "Phải nhập lại mật khẩu";
            else if (matkhau != nhaplaimk)
                ViewData["Loi8"] = "Nhập lại sai mật khẩu";
            else if (String.IsNullOrEmpty(email))
                ViewData["Loi5"] = "Phải nhập email";
            else if (String.IsNullOrEmpty(diachi))
                ViewData["Loi6"] = "Phải nhập địa chỉ";
            else if (String.IsNullOrEmpty(dienthoai))
                ViewData["Loi7"] = "Phải nhập SĐT";
            else
            {
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = matkhau;
                kh.Email = email;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            return this.Dangky();
        }
        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            if (string.IsNullOrEmpty(tendn))
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            else if (string.IsNullOrEmpty(matkhau))
                ViewData["Loi2"] = " Phải nhập mật khẩu";
            else
            {
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == matkhau);
                if (kh != null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    Session["Name"] = data.KHACHHANGs.FirstOrDefault(i=>i.Taikhoan==kh.Taikhoan).HoTen;
                    return RedirectToAction("Index", "Home");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult ThongTin()
        {
            if (Session["Taikhoan"] == null)
                return RedirectToAction("Dangnhap");
            KHACHHANG khachHang = Session["Taikhoan"] as KHACHHANG;
            return View(khachHang);
        }
        public ActionResult ThongTinVeChungToi()
        {
            return View();
        }
        public ActionResult Dangxuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult EditThongTin()
        {
            if (Session["Taikhoan"] == null)
                return RedirectToAction("Dangnhap");
            KHACHHANG khachHang = Session["Taikhoan"] as KHACHHANG;
            return View(khachHang);
        }
        [HttpPost]
        public ActionResult EditThongTin(KHACHHANG khachHang)
        {
            if (Session["Taikhoan"] == null)
                return RedirectToAction("Dangnhap");
            KHACHHANG khachHangSession = Session["Taikhoan"] as KHACHHANG;
            using (ShopDataDataContext data = new ShopDataDataContext())
            {
                KHACHHANG khachHangDB = data.KHACHHANGs.SingleOrDefault(kh => kh.MaKH == khachHangSession.MaKH);
                if (khachHangDB == null)
                    return HttpNotFound();
                khachHangDB.HoTen = khachHang.HoTen;
                khachHangDB.Email = khachHang.Email;
                khachHangDB.DiachiKH = khachHang.DiachiKH;
                khachHangDB.DienthoaiKH = khachHang.DienthoaiKH;
                khachHangDB.Ngaysinh = khachHang.Ngaysinh;
                data.SubmitChanges();
            }
            Session["Taikhoan"] = khachHang;
            return RedirectToAction("ThongTin");
        }
        public ActionResult DoiMatKhau()
        {
            if (Session["Taikhoan"] == null)
                return RedirectToAction("Dangnhap");
            KHACHHANG khachHang = Session["Taikhoan"] as KHACHHANG;
            return View(khachHang);
        }
        [HttpPost]
        public ActionResult DoiMatKhau(string currentPassword, string newPassword, string confirmNewPassword)
        {
            using (var data = new ShopDataDataContext())
            {
                KHACHHANG khachHang = Session["Taikhoan"] as KHACHHANG;
                var user = data.KHACHHANGs.SingleOrDefault(u => u.Taikhoan == khachHang.Taikhoan);
                if (user != null && user.Matkhau == currentPassword)
                {
                    if (newPassword == confirmNewPassword)
                    {
                        user.Matkhau = newPassword;
                        data.SubmitChanges();
                        TempData["PasswordChanged"] = "true";
                        return RedirectToAction("DoiMatKhau");
                    }
                    else
                        TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận mật khẩu mới không trùng khớp.";
                }
                else
                    TempData["ErrorMessage"] = "Mật khẩu hiện tại không chính xác.";
            }
            return RedirectToAction("DoiMatKhau");
        }
    }
}