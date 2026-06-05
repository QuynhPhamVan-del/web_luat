
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web_luat.Models;

namespace web_luat.Areas.Admin.Controllers
{
    public class QuanTriController : Controller
    {
        dbcontext db = new dbcontext();
        // GET: Admin/QuanTri
        public ActionResult Index(int Language = 0)
        {
            return View();
        }
        public ActionResult Login()
        {
            if (Session["HoTen"] != null)
            {
                return Redirect("/Admin/QuanTri/Index");
            }
            return View();
        }
        [HttpPost]
        public JsonResult Login(string Tendangnhap, string MatKhau)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { status = false, message = "Thông tin đăng nhập không hợp lệ." });
                }

                var user = db.tbl_User.FirstOrDefault(u => u.UserName == Tendangnhap);
                string token = Guid.NewGuid().ToString();

                // lưu DB
                user.LoginToken = token;
                db.tbl_User.AddOrUpdate(user);
                db.SaveChanges();
                var context = GlobalHost.ConnectionManager.GetHubContext<AuthHub>();

                if (AuthHub.UserConnections.TryGetValue(user.ID, out string oldConnectionId))
                {
                    context.Clients.Client(oldConnectionId).forceLogout();
                }
                if (user == null || !PasswordHelper.VerifyPassword(MatKhau, user.Password))
                {
                    return Json(new { status = false, message = "Sai tài khoản hoặc mật khẩu." });
                }

                // ✅ LƯU SESSION CHUẨN (KHÔNG LƯU OBJECT)
                Session["UserId"] = user.ID;
                Session["UserName"] = user.UserName;
                Session["FullName"] = user.FullName;
                Session["IdDV"] = user.IdDonVi;
                Session["HinhAnh"] = user.Image;
                Session["HoTen"] = user.FullName;
                Session["Id"] = user.ID;
                var idNhomQuyen = db.tbl_User_NhomQuyen.FirstOrDefault(g => g.IdUser == user.ID).IdNhomQuyen;
                var admin = db.tbl_NhomQuyen.Find(idNhomQuyen).IsAdmin == true ? 1 : 0;
                Session["Admin"] = admin;
                // ✅ URL CHUẨN AREA
                var url = Url.Action("Index", "QuanTri", new { area = "Admin" });

                return Json(new
                {
                    status = true,
                    message = "Đăng nhập thành công",
                    redirect = url
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "Lỗi hệ thống: " + ex.Message
                });
            }
        }
        //public PartialViewResult ThongTin()
        //{
        //    ThongTin banner = db.ThongTins.First();
        //    if(banner == null)
        //    {
        //        banner = new ThongTin();
        //    }
        //    return PartialView(banner);
        //}
        public ActionResult DoiMK(int? Id)
        {
            tbl_User banner = db.tbl_User.FirstOrDefault(g => g.ID == Id);
            if (banner == null)
            {
                banner = new tbl_User();
            }
            return PartialView(banner);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult DoiMK(string PassCu, int Id, string PassMoi, string PassXN)
        {
            try
            {
                tbl_User banner = db.tbl_User.FirstOrDefault(g => g.ID == Id);
                if (string.IsNullOrEmpty(PassCu) || string.IsNullOrEmpty(PassXN) || string.IsNullOrEmpty(PassMoi))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (!PasswordHelper.VerifyPassword(PassCu, banner.Password))
                {
                    return Json(new { status = false, message = "Mật khẩu cũ không chính xác." }, JsonRequestBehavior.AllowGet);
                }
                if (PassMoi != PassXN)
                {
                    return Json(new { status = false, message = "Mật khẩu xác nhận không trùng khớp." }, JsonRequestBehavior.AllowGet);
                }
                banner.Password = PasswordHelper.HashPassword(PassMoi);
                db.tbl_User.AddOrUpdate(banner);
                db.SaveChanges();
                return Json(new { status = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Logout()
        {
            int? userId = Session["UserId"] as int?;

            if (userId != null)
            {
                using (var db = new dbcontext())
                {
                    var user = db.tbl_User.FirstOrDefault(x => x.ID == userId);
                    if (user != null)
                    {
                        user.LoginToken = null;
                        db.SaveChanges();
                    }
                }
            }

            Session.Clear();
            return RedirectToAction("Login");
        }
        public ActionResult Menu2()
        {
            var idUser = Convert.ToInt32(Session["UserId"].ToString());
            var lst = db.LoadMenus.Where(g => g.IdUser == idUser).ToList();
            ViewData["lst"] = lst;
            return View();
        }
    }
}