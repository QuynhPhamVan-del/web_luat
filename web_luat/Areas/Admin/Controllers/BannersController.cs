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
    public class BannersController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, string keyword, bool isTrash = false, int IdDonVi = 0)
        {
            var id = Convert.ToInt32(Session["IdDV"].ToString());
            IdDonVi = id;
            var query = db.tbl_Banner.AsQueryable();

            query = query.Where(g => g.IdDonVi == IdDonVi);

            // lọc theo trạng thái
            query = isTrash
                ? query.Where(x => x.IsDelete == true)
                : query.Where(x => x.IsDelete == false);


            int pageSize = 15;
            int pageNumber = (page ?? 1);

            ViewBag.pageNumber = page;
            ViewBag.total = query.Count();
            ViewBag.keyword = keyword;
            ViewBag.isTrash = isTrash;
            ViewBag.IdDonVi = IdDonVi;
            return View(query.OrderBy(x => x.STT).ToPagedList(pageNumber, pageSize));
        }

        public PartialViewResult Create(int IdDonVi = 0)
        {
            ViewBag.IdDonVi = IdDonVi;
            return PartialView();
        }
        [HttpPost]
        public JsonResult Create([Bind(Include = "Id,Image,Link,STT,IdDonVi")] tbl_Banner banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (f == null || banner.STT == null)
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/Banner/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/Banner/" + FileName;
                }

                banner.IsDelete = false;
                db.tbl_Banner.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới banner ";
                a.Controller = "Banners";
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
            tbl_Banner banner = db.tbl_Banner.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id,Image,Link,STT,IdDonVi")] tbl_Banner banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (banner.STT == null)
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~" + banner.Image);
                    if (System.IO.File.Exists(folderPath))
                    {
                        System.IO.File.Delete(folderPath);
                    }
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/Banner/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/Banner/" + FileName;
                }
                banner.IsDelete = false;
                db.tbl_Banner.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa banner ";
                a.Controller = "Banners";
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
                return Json(new { status = true, message = "Banner không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_Banner banner = db.tbl_Banner.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Banner đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            banner.IsDelete = true;
            db.tbl_Banner.AddOrUpdate(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa banner ";
            a.Controller = "Banners";
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
                return Json(new { status = true, message = "Banner không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_Banner banner = db.tbl_Banner.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Banner đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            banner.IsDelete = false;
            db.tbl_Banner.AddOrUpdate(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Khôi phục banner ";
            a.Controller = "Banners";
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
                return Json(new { status = true, message = "Banner không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_Banner banner = db.tbl_Banner.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Banner đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            db.tbl_Banner.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa vĩnh viễn banner ";
            a.Controller = "Banners";
            a.Action = "DeleteTVC";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa vĩnh viễn bản ghi." }, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}