using Antlr.Runtime;
using PagedList;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class AdminController : Controller
    {
        ShopDataDataContext data = new ShopDataDataContext();
        // GET: Admin
        private List<SANPHAM> Themmoisanpham(int count)
        {
            return data.SANPHAMs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
                return View();
        }
        public ActionResult SanPham(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.SANPHAMs.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var tendn = f["txtUser"];
            var matkhau = f["txtPass"];
            if (String.IsNullOrEmpty(tendn))
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            else if (String.IsNullOrEmpty(matkhau))
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            else
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Themmoisanpham()
        {
            ViewBag.MaL = new SelectList(data.LOAIs.ToList().OrderBy(n => n.TenLoai), "MaL", "TenLoai");
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisanpham(SANPHAM sanpham, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaL = new SelectList(data.LOAIs.ToList().OrderBy(n => n.TenLoai), "MaL", "TenLoai", sanpham.MaL);
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sanpham.MaNCC);
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("/assets/images/sanpham"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                        fileUpload.SaveAs(path);
                    sanpham.Anhbia = "/assets/images/sanpham/" + fileName;
                    data.SANPHAMs.InsertOnSubmit(sanpham);
                    data.SubmitChanges();
                }
                return RedirectToAction("SanPham", "Admin");
            }
        }
        public ActionResult Chitietsanpham(int id)
        {
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sanpham);
        }
        [HttpGet]
        public ActionResult Xoasanpham(int id)
        {
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(sanpham);
        }
        [HttpPost, ActionName("Xoasanpham")]
        public ActionResult Xacnhanxoa(int id)
        {
            SANPHAM sanpham = data.SANPHAMs.Select(p => p).Where(p => p.MaSP == id).FirstOrDefault();
            ViewBag.MaL = sanpham.MaL;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SANPHAMs.DeleteOnSubmit(sanpham);
            data.SubmitChanges();
            return RedirectToAction("SanPham");
        }
        [HttpGet]
        public ActionResult Suasanpham(int id)
        {
            SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MaSP == id);
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaL = new SelectList(data.LOAIs.ToList().OrderBy(n => n.TenLoai), "MaL", "TenLoai", sanpham.MaL);
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sanpham.MaNCC);
            return View(sanpham);
        }
        [HttpPost, ActionName("Suasanpham")]
        [ValidateInput(false)]
        public ActionResult Suasanpham(SANPHAM sanpham, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaL = new SelectList(data.LOAIs.ToList().OrderBy(n => n.TenLoai), "MaL", "TenLoai");
            ViewBag.MaNCC = new SelectList(data.NHACUNGCAPs.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(sanpham);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("/assets/images/sanpham/"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại !";
                    else
                        fileUpload.SaveAs(path);
                    sanpham.Anhbia = fileName;
                    SANPHAM sp = data.SANPHAMs.SingleOrDefault(s => s.MaSP == sanpham.MaSP);
                    sp.TenSP = sanpham.TenSP;
                    sp.Giaban = sanpham.Giaban;
                    sp.Mota = sanpham.Mota;
                    sp.Anhbia = "/assets/images/sanpham/" + sanpham.Anhbia;
                    sp.Ngaycapnhat = sp.Ngaycapnhat;
                    sp.Soluongton = sp.Soluongton;
                    sp.MaNCC = sanpham.MaNCC;
                    sp.MaL = sanpham.MaL;
                    data.SubmitChanges();
                }
                return RedirectToAction("SanPham");
            }
        }
        public ActionResult HoaDon()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                var hoadon = from hd in data.DONDATHANGs
                             orderby hd.Ngaydat descending
                             select hd;
                return View(hoadon);
            }
        }
        public ActionResult ChiTietHoaDon(int id)
        {
            var find = data.DONDATHANGs.First(p => p.MaDonHang == id);
            if (find != null)
            {
                var cthd = data.CHITIETDONTHANGs.Where(p => p.MaDonHang == id).ToList();
                ViewBag.cthd = cthd;
                return View(find);
            }
            return View("Index", "Home");
        }
        public ActionResult EditHoaDon(int id)
        {
            var hoaDon = data.DONDATHANGs.FirstOrDefault(p => p.MaDonHang == id);
            if (hoaDon != null)
            {
                ViewBag.TinhTrangGiaoHang = new SelectList(new[]
                {
            new { Value = "giao that bai", Text = "Giao thất bại" },
            new { Value = "giao thanh cong", Text = "Giao thành công" },
            new { Value = "dang giao", Text = "Đang giao" }
        }, "Value", "Text", hoaDon.Tinhtranggiaohang);

                return View(hoaDon);
            }

            return View("HoaDon");
        }
        [HttpPost]
        public ActionResult SaveHoaDon(DONDATHANG hoaDon)
        {
            if (ModelState.IsValid)
            {
                var existingHoaDon = data.DONDATHANGs.FirstOrDefault(p => p.MaDonHang == hoaDon.MaDonHang);
                if (existingHoaDon != null)
                {
                    existingHoaDon.Tinhtranggiaohang = hoaDon.Tinhtranggiaohang;
                    data.SubmitChanges();
                    return RedirectToAction("HoaDon");
                }
            }

            return View("HoaDon");
        }
    }
}