using web_luat.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web_luat.Areas.Admin.Controllers
{
    [AdminAuthorize]

    public class ThuVienConController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult IndexTV(int? page, int Id)
        {
            ViewBag.Id = Id;
            var list = db.tbl_ThuVienAnh_Album.Where(g => g.IdThuVienImage == Id).ToList();
            if (page == null) page = 1;
            var books = list.OrderBy(g => g.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.pageNumber = page;
            ViewBag.total = list.ToList().Count();
            return View(books.ToPagedList(pageNumber, pageSize));
        }
        public PartialViewResult CreateTVC(int? Id)
        {
            ViewBag.Id = Id;
            return PartialView();
        }
        [HttpPost]
        public JsonResult CreateTVC([Bind(Include = "Id,IdThuVienImage,MoTa,FileImage")] tbl_ThuVienAnh_Album banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (f == null || string.IsNullOrEmpty(banner.MoTa))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/ThuVienAnh/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.FileImage = "/Upload/ThuVienAnh/" + FileName;
                }
                banner.IsDelete = false;
                db.tbl_ThuVienAnh_Album.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới thư viện ảnh con" + banner.MoTa;
                a.Controller = "ThuVienAnh";
                a.Action = "CreateTVC";
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
        public PartialViewResult EditTVC(int? id)
        {
            tbl_ThuVienAnh_Album banner = db.tbl_ThuVienAnh_Album.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult EditTVC([Bind(Include = "Id,IdThuVienImage,MoTa,FileImage")] tbl_ThuVienAnh_Album banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (string.IsNullOrEmpty(banner.MoTa))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {

                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/ThuVienAnh/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.FileImage = "/Upload/ThuVienAnh/" + FileName;
                }
                banner.IsDelete = false;
                db.tbl_ThuVienAnh_Album.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa thư viện ảnh con" + banner.MoTa;
                a.Controller = "ThuVienAnh";
                a.Action = "EditTVC";
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
            tbl_ThuVienAnh_Album banner = db.tbl_ThuVienAnh_Album.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Banner đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }

            db.tbl_ThuVienAnh_Album.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa thư viện ảnh con" + banner.MoTa;
            a.Controller = "ThuVienAnh";
            a.Action = "DeleteTVC";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
        }
    }
}