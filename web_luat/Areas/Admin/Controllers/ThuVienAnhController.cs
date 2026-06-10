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

    public class ThuVienAnhController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page)
        {
            var IsLanguage = Convert.ToInt32(Session["Language"]);
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "QuanTri", new { area = "Admin" });
            }
            var list = db.tbl_ThuVienAnh.OrderBy(g => g.STT).ToList();
            if (page == null) page = 1;
            var books = list.OrderBy(g => g.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.pageNumber = page;
            ViewBag.total = list.ToList().Count();
            return View(books.ToPagedList(pageNumber, pageSize));
        }

        public PartialViewResult Create()
        {

            return PartialView();
        }
        [HttpPost]
        public JsonResult Create([Bind(Include = "Id,Image,TieuDe,STT,IdDonVi")] tbl_ThuVienAnh banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (f == null)
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/ThuVienAnh/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/ThuVienAnh/" + FileName;
                }
                db.tbl_ThuVienAnh.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới thư viện ảnh ";
                a.Controller = "ThuVienAnh";
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
            tbl_ThuVienAnh banner = db.tbl_ThuVienAnh.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id,Image,TieuDe,STT,IdDonVi")] tbl_ThuVienAnh banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (f != null && f.ContentLength > 0)
                {

                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/ThuVienAnh/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/ThuVienAnh/" + FileName;
                }
                db.tbl_ThuVienAnh.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa thư viện ảnh ";
                a.Controller = "ThuVienAnh";
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
            tbl_ThuVienAnh banner = db.tbl_ThuVienAnh.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Banner đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }

            db.tbl_ThuVienAnh.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa thư viện ảnh ";
            a.Controller = "ThuVienAnh";
            a.Action = "Delete";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
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