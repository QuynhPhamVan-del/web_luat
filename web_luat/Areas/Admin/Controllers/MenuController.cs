using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web_luat.Models;

namespace web_luat.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class MenuController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, int IdChuyenMuc = 0,  bool isTrash = false, string Key = "")
        {
            var list = db.tbl_MenuPage.Where(g =>  g.IsDelete == isTrash);
            if (IdChuyenMuc > 0)
            {
                list = list.Where(g => g.IdChuyenMuc == IdChuyenMuc);
            }
            if (!string.IsNullOrEmpty(Key))
            {
                list = list.Where(g => g.TenDonVi.Trim().Contains(Key.Trim()));
            }
            if (page == null) page = 1;
            var books = list.OrderBy(g => g.Id);
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            ViewBag.pageNumber = page;
            ViewBag.total = list.ToList().Count();
            ViewData["menu"] = db.tbl_MenuPage.ToList();
            ViewData["chuyenmuc"] = db.tbl_ChuyenMuc.ToList();

            ViewBag.IdChuyenMuc = IdChuyenMuc;
 
            ViewBag.isTrash = isTrash;
            ViewBag.Key = Key;
            return View(books.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult LoadCM(int IdDonVi)
        {
            var lstIdCM = db.tbl_DonViChuyenMuc.Where(g => g.IdDonVi == IdDonVi).OrderBy(g => g.STT).ToList();
            List<tbl_ChuyenMuc> lstCM = new List<tbl_ChuyenMuc>();
            var IsLanguage = Convert.ToInt32(Session["Language"]);
            if (IdDonVi == 0)
            {
                lstCM = db.tbl_ChuyenMuc.ToList();

            }
            if (lstIdCM.Count() > 0)
            {
                var id = lstIdCM.Select(g => g.IdChuyenMuc).ToList();
                lstCM = db.tbl_ChuyenMuc.AsQueryable().Where(g => id.Contains(g.Id)).ToList();
            }
            ViewData["lstCM"] = lstCM;
            return View();
        }
        public ActionResult LoadCMEdit(int IdDonVi, int Id)
        {
            var IsLanguage = Convert.ToInt32(Session["Language"]);
            var lstIdCM = db.tbl_DonViChuyenMuc.Where(g => g.IdDonVi == IdDonVi).OrderBy(g => g.STT).ToList();
            List<tbl_ChuyenMuc> lstCM = new List<tbl_ChuyenMuc>();
            if (IdDonVi == 0)
            {
                lstCM = db.tbl_ChuyenMuc.ToList();

            }
            if (lstIdCM.Count() > 0)
            {
                var id = lstIdCM.Select(g => g.IdChuyenMuc).ToList();
                lstCM = db.tbl_ChuyenMuc.AsQueryable().Where(g => id.Contains(g.Id)).ToList();
            }
            ViewBag.Id = Id;
            ViewData["lstCM"] = lstCM;
            return View();
        }
        public PartialViewResult Create()
        {
            ViewData["menu"] = db.tbl_MenuPage.AsQueryable().Where(g => g.IsDelete == false ).ToList();
            ViewData["chuyenmuc"] = db.tbl_ChuyenMuc.ToList();
            return PartialView();
        }
        [HttpPost]
        public JsonResult Create([Bind(Include = "Id,TenDonVi,IdCha,STT,Link,IdChuyenMuc,IdBaiViet,IdLoaiVB,IdDonVi")] tbl_MenuPage banner)
        {
            try
            {
                var IsLanguage = Convert.ToInt32(Session["Language"]);
                if (string.IsNullOrEmpty(banner.TenDonVi) || string.IsNullOrEmpty(banner.STT.ToString()))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);

                }
                banner.TrenDuoi = Convert.ToInt32(Session["trenduoi"]);
                banner.IdBaiViet = null;
                banner.IsDelete = false;
                db.tbl_MenuPage.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới menu  " + banner.TenDonVi;
                a.Controller = "Menu";
                a.Action = "Create";
                a.NgayThucHien = DateTime.Now;
                a.IdUser = Convert.ToInt32(Session["Id"].ToString());
                db.tbl_Log.Add(a);
                db.SaveChanges();
                return Json(new { status = true, message = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Banners/Edit/5
        public PartialViewResult Edit(int? id)
        {
            ViewData["menu"] = db.tbl_MenuPage.AsQueryable().Where(g => g.IsDelete == false ).ToList();
            ViewData["chuyenmuc"] = db.tbl_ChuyenMuc.ToList();
            tbl_MenuPage banner = db.tbl_MenuPage.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id,TenDonVi,IdCha,STT,Link,IdChuyenMuc,IdBaiViet,IdLoaiVB,IdDonVi")] tbl_MenuPage banner)
        {
            try
            {
                var IsLanguage = Convert.ToInt32(Session["Language"]);
                if (string.IsNullOrEmpty(banner.TenDonVi) || string.IsNullOrEmpty(banner.STT.ToString()))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);

                }
                banner.TrenDuoi = Convert.ToInt32(Session["trenduoi"]);
                banner.IsDelete = false;
                db.tbl_MenuPage.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa menu  " + banner.TenDonVi;
                a.Controller = "Menu";
                a.Action = "Edit";
                a.NgayThucHien = DateTime.Now;
                a.IdUser = Convert.ToInt32(Session["Id"].ToString());
                db.tbl_Log.Add(a);
                db.SaveChanges();
  
                return Json(new { status = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
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
                return Json(new { status = true, message = "Không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_MenuPage banner = db.tbl_MenuPage.Find(id);
            banner.IsDelete = true;
            db.tbl_MenuPage.AddOrUpdate(banner);
            db.SaveChanges();
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa menu  " + banner.TenDonVi;
            a.Controller = "Menu";
            a.Action = "Delete";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();

            return Json(new { status = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult btnRestoreSingle(int? id)
        {
            if (id == null)
            {
                return Json(new { status = true, message = "Menu không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_MenuPage banner = db.tbl_MenuPage.Find(id);
            banner.IsDelete = false;
            db.tbl_MenuPage.AddOrUpdate(banner);
            db.SaveChanges();
            tbl_Log a = new tbl_Log();
            a.MoTa = "Khôi phục menu: " + banner.TenDonVi;
            a.Controller = "Menu";
            a.Action = "btnRestoreSingle";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();

            return Json(new { status = true, message = "Khôi phục thành công." }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTVC(int? id)
        {
            if (id == null)
            {
                return Json(new { status = true, message = "Menu không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_MenuPage banner = db.tbl_MenuPage.Find(id);
            db.tbl_MenuPage.Remove(banner);
            db.SaveChanges();
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa vĩnh viễn menu " + banner.TenDonVi;
            a.Controller = "Menu";
            a.Action = "DeleteTVC";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa vĩnh viễn bản ghi." }, JsonRequestBehavior.AllowGet);
        }
    }
}