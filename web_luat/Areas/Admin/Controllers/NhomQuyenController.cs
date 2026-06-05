
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
    public class NhomQuyenController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, string Keyword = "", bool isTrash = false)
        {
            var list = db.tbl_NhomQuyen.AsQueryable().Where(g => g.IsDelete == isTrash);
            if (!string.IsNullOrEmpty(Keyword))
            {
                list = list.Where(g => g.TenNhomQuyen.Contains(Keyword));
            }
            if (page == null) page = 1;
            var books = list.OrderBy(g => g.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.pageNumber = page;
            ViewBag.total = list.ToList().Count();
            ViewBag.key = Keyword;
            ViewBag.isTrash = isTrash;
            return View(books.ToPagedList(pageNumber, pageSize));
        }

        public PartialViewResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult Create([Bind(Include = "Id,TenNhomQuyen")] tbl_NhomQuyen banner)
        {
            try
            {
                if (string.IsNullOrEmpty(banner.TenNhomQuyen))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);

                }
                banner.IsDelete = false;
                banner.IsAdmin = Request["IsAdmin"] == "on" ? true : false;
                db.tbl_NhomQuyen.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới nhóm quyền  " + banner.TenNhomQuyen;
                a.Controller = "NhomQuyen";
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
            tbl_NhomQuyen banner = db.tbl_NhomQuyen.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id,TenNhomQuyen")] tbl_NhomQuyen banner)
        {
            try
            {
                if (string.IsNullOrEmpty(banner.TenNhomQuyen))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);

                }
                banner.IsDelete = false;
                banner.IsAdmin = Request["IsAdmin"] == "on" ? true : false;
                db.tbl_NhomQuyen.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa nhóm quyền  " + banner.TenNhomQuyen;
                a.Controller = "NhomQuyen";
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
                return Json(new { status = true, message = "Nhóm quyền không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_NhomQuyen banner = db.tbl_NhomQuyen.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Nhóm quyền đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            banner.IsDelete = false;
            banner.IsDelete = true;
            db.tbl_NhomQuyen.AddOrUpdate(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa nhóm quyền ";
            a.Controller = "NhomQuyen";
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
                return Json(new { status = true, message = "Nhóm quyền không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_NhomQuyen banner = db.tbl_NhomQuyen.FirstOrDefault(g => g.Id == id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Nhóm quyền đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            banner.IsDelete = false;
            db.tbl_NhomQuyen.AddOrUpdate(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Khôi phục nhóm quyền  ";
            a.Controller = "NhomQuyen";
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
                return Json(new { status = true, message = "Nhóm quyền không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_NhomQuyen banner = db.tbl_NhomQuyen.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Nhóm quyền đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            db.tbl_NhomQuyen.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa vĩnh viễn nhóm quyền  ";
            a.Controller = "NhomQuyen";
            a.Action = "DeleteTVC";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa vĩnh viễn bản ghi." }, JsonRequestBehavior.AllowGet);
        }
    }
}