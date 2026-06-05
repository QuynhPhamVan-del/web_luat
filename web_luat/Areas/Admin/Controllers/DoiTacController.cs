
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
    public class DoiTacController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, string keyword, bool isTrash = false)
        {
            var query = db.tbl_DoiTac.AsQueryable().Where(x => x.IsDelete == isTrash);

            // tìm kiếm theo tên đối tác
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(g => g.TenDoiTac.ToLower().Contains(keyword.ToLower()));
            }

            if (page == null) page = 1;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.pageNumber = page;
            ViewBag.total = query.Count();
            ViewBag.keyword = keyword;
            ViewBag.isTrash = isTrash;
            var list = query.OrderBy(g => g.Id);

            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public PartialViewResult Create()
        {

            return PartialView();
        }
        [HttpPost]
        public JsonResult Create([Bind(Include = "Id,TenDoiTac,AnhDaiDien,Link")] tbl_DoiTac banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (f == null || string.IsNullOrEmpty(banner.TenDoiTac))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/DoiTac/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.AnhDaiDien = "/Upload/DoiTac/" + FileName;
                }
                banner.IsDelete = false;
                db.tbl_DoiTac.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới đối tác ";
                a.Controller = "DoiTac";
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
            tbl_DoiTac banner = db.tbl_DoiTac.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id,TenDoiTac,AnhDaiDien,Link")] tbl_DoiTac banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/DoiTac/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.AnhDaiDien = "/Upload/DoiTac/" + FileName;
                }
                banner.IsDelete = false;
                db.tbl_DoiTac.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa đối tác ";
                a.Controller = "DoiTac";
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
                return Json(new { status = true, message = "Đối tác không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_DoiTac banner = db.tbl_DoiTac.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Đối tác đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            banner.IsDelete = true;
            db.tbl_DoiTac.AddOrUpdate(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa Đối tác ";
            a.Controller = "DoiTac";
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
                return Json(new { status = true, message = "Đối tác không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_DoiTac banner = db.tbl_DoiTac.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Đối tác đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            banner.IsDelete = false;
            db.tbl_DoiTac.AddOrUpdate(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Khôi phục đối tác ";
            a.Controller = "DoiTac";
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
                return Json(new { status = true, message = "Đối tác không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_DoiTac banner = db.tbl_DoiTac.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Đối tác đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            db.tbl_DoiTac.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa vĩnh viễn Đối tác ";
            a.Controller = "DoiTac";
            a.Action = "DeleteTVC";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa vĩnh viễn bản ghi." }, JsonRequestBehavior.AllowGet);
        }
    }
}