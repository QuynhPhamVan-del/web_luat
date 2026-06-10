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
    public class LienKetKhoaController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, bool isTrash = false)
        {
            var query = db.tbl_LienKetKhoa.Where(g => g.IsDelete == isTrash);

            // tìm kiếm tiêu đề
            if (page == null) page = 1;

            int pageSize = 15;
            int pageNumber = (page ?? 1);

            ViewBag.pageNumber = page;
            ViewBag.total = query.Count();
            ViewBag.isTrash = isTrash;
            var list = query.OrderBy(g => g.Id);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public PartialViewResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Create([Bind(Include = "Id,GioiThieu,Icon,TieuDe,Link,STT")] tbl_LienKetKhoa banner)
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
                    string UploadPath = Server.MapPath("~/Upload/DichVu/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Icon = "/Upload/DichVu/" + FileName;
                }
                banner.IsDelete = false;
                db.tbl_LienKetKhoa.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới dịch vụ của chúng tôi" + banner.TieuDe;
                a.Controller = "LienKetKhoa";
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

            tbl_LienKetKhoa banner = db.tbl_LienKetKhoa.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit([Bind(Include = "Id,GioiThieu,Icon,TieuDe,Link,STT")] tbl_LienKetKhoa banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/DichVu/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Icon = "/Upload/DichVu/" + FileName;
                }
                banner.IsDelete = false;
                db.tbl_LienKetKhoa.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa dịch vụ của chúng tôi";
                a.Controller = "LienKetKhoa";
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
                return Json(new { status = true, message = "liên kết khoa không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_LienKetKhoa banner = db.tbl_LienKetKhoa.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "liên kết khoa đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            banner.IsDelete = true;
            db.tbl_LienKetKhoa.AddOrUpdate(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa dịch vụ của chúng tôi";
            a.Controller = "LienKetKhoa";
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
                return Json(new { status = true, message = "liên kết khoa không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_LienKetKhoa banner = db.tbl_LienKetKhoa.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "liên kết khoa đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            banner.IsDelete = false;
            db.tbl_LienKetKhoa.AddOrUpdate(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Khôi phục dịch vụ của chúng tôi " ;
            a.Controller = "LienKetKhoa";
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
                return Json(new { status = true, message = "liên kết khoa không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_LienKetKhoa banner = db.tbl_LienKetKhoa.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "liên kết khoa đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            db.tbl_LienKetKhoa.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa vĩnh viễn dịch vụ của chúng tôi";
            a.Controller = "LienKetKhoa";
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