using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using web_luat.Models;

namespace web_luat.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class NguoiDungController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page)
        {

            var list = db.tbl_User.ToList();
            if (page == null) page = 1;
            var books = list.OrderBy(g => g.ID);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.pageNumber = page;
            ViewBag.total = list.ToList().Count();
            return View(books.ToPagedList(pageNumber, pageSize));
        }

        public PartialViewResult Create()
        {
            var IsLanguage = Convert.ToInt32(Session["Language"]);
            return PartialView();
        }
        [HttpPost]
        public JsonResult Create([Bind(Include = "ID,UserName,Password,FullName,Email,Image,IdDonVi")] tbl_User banner)
        {
            try
            {

                if (string.IsNullOrEmpty(banner.UserName) || string.IsNullOrEmpty(banner.Password) || string.IsNullOrEmpty(banner.FullName) || string.IsNullOrEmpty(banner.Email))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }

                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(banner.Email, pattern, RegexOptions.IgnoreCase))
                {
                    return Json(new { status = false, message = "Email không đúng định dạng." }, JsonRequestBehavior.AllowGet);
                }
       
                banner.Password = PasswordHelper.HashPassword(banner.Password);
                db.tbl_User.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới tài khoản " + banner.FullName;
                a.Controller = "NguoiDung";
                a.Action = "Create";
                a.NgayThucHien = DateTime.Now;
                a.IdUser = Convert.ToInt32(Session["Id"].ToString());
                db.tbl_Log.Add(a);
                db.SaveChanges();
                return Json(new { status = true, message = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }

        //// GET: Admin/Banners/Edit/5
        public PartialViewResult Edit(int? id)
        {
            var IsLanguage = Convert.ToInt32(Session["Language"]);
            tbl_User banner = db.tbl_User.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult Edit([Bind(Include = "ID,UserName,Password,FullName,Email,Image,IdDonVi")] tbl_User banner)
        {
            try
            {
                if (string.IsNullOrEmpty(banner.UserName) || string.IsNullOrEmpty(banner.Password) || string.IsNullOrEmpty(banner.FullName) || string.IsNullOrEmpty(banner.Email))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }

                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(banner.Email, pattern, RegexOptions.IgnoreCase))
                {
                    return Json(new { status = false, message = "Email không đúng định dạng." }, JsonRequestBehavior.AllowGet);
                }
                var f = Request.Files["file"];
          
                banner.Password = PasswordHelper.HashPassword(banner.Password);
                db.tbl_User.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa tài khoản " + banner.FullName;
                a.Controller = "NguoiDung";
                a.Action = "Edit";
                a.NgayThucHien = DateTime.Now;
                a.IdUser = Convert.ToInt32(Session["Id"].ToString());
                db.tbl_Log.Add(a);
                db.SaveChanges();
                return Json(new { status = true, message = "Sửa thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { status = true, message = "Người dùng không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_User banner = db.tbl_User.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Người đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            db.tbl_User.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa tài khoản " + banner.FullName;
            a.Controller = "NguoiDung";
            a.Action = "Delete";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult AddQuyen(int IdNhomQuyen)
        {
            ViewBag.IdNhomQuyen = IdNhomQuyen;
            ViewData["lstMenu"] = db.tbl_MenuAdmin.ToList();
            ViewData["lstQuyen"] = db.tbl_Menu_User.Where(g => g.IdNhomQuyen == IdNhomQuyen).ToList();
            return PartialView();
        }
        [HttpPost]
        public JsonResult AddQuyen(int IdMenu, int IdNhomQuyen, bool Check)
        {
            try
            {
                if (Check == true)
                {
                    var quyen = new tbl_Menu_User();
                    quyen.IdNhomQuyen = IdNhomQuyen;
                    quyen.IdMenu = IdMenu;
                    db.tbl_Menu_User.Add(quyen);
                    db.SaveChanges();
                }
                else
                {
                    var check = db.tbl_Menu_User.FirstOrDefault(g => g.IdNhomQuyen == IdNhomQuyen && g.IdMenu == IdMenu);
                    db.tbl_Menu_User.Remove(check);
                    db.SaveChanges();
                }

                return Json(new { status = true, message = "Update quyền thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult AddNhomQuyen(int IdUser)
        {
            ViewBag.IdUser = IdUser;
            ViewData["lstNhomQuyen"] = db.tbl_NhomQuyen.ToList();
            ViewData["lstUserNhomQuyen"] = db.tbl_User_NhomQuyen.Where(g => g.IdUser == IdUser).ToList();
            return PartialView();
        }
        [HttpPost]
        public JsonResult AddNhomQuyen(int IdNhomQuyen, int IdUser, bool Check)
        {
            try
            {
                if (Check == true)
                {
                    var quyen = new tbl_User_NhomQuyen();
                    quyen.IdUser = IdUser;
                    quyen.IdNhomQuyen = IdNhomQuyen;
                    db.tbl_User_NhomQuyen.Add(quyen);
                    db.SaveChanges();
                }
                else
                {
                    var check = db.tbl_User_NhomQuyen.FirstOrDefault(g => g.IdUser == IdUser && g.IdNhomQuyen == IdNhomQuyen);
                    db.tbl_User_NhomQuyen.Remove(check);
                    db.SaveChanges();
                }

                return Json(new { status = true, message = "Update quyền thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}