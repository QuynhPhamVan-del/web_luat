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
    public class GioiThieuController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page)
        {
            var list = db.tbl_GioiThieu.ToList();
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
            ViewData["donvi"] = db.tbl_DonVi.ToList();
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Create([Bind(Include = "Id,TieuDeChinh,NoiDung,Image,Link,,Bac,Mau,IdDonVi,TieuDePhu")] tbl_GioiThieu banner)
        {
            try
            {
                var f = Request.Files["file"];
                var iddonvi = Request["IdDonVi"];
                banner.IdDonVi = Convert.ToInt32(iddonvi);
                if (f == null)
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/GioiThieu/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/GioiThieu/" + FileName;
                }
                db.tbl_GioiThieu.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới thông tin giới thiệu " + banner.TieuDeChinh;
                a.Controller = "GioiThieu";
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
            tbl_GioiThieu banner = db.tbl_GioiThieu.Find(id);
            ViewData["donvi"] = db.tbl_DonVi.ToList();
            return PartialView(banner);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit([Bind(Include = "Id,TieuDeChinh,NoiDung,Image,Link,IdDonVi,Bac,Mau,TieuDePhu")] tbl_GioiThieu banner)
        {
            try
            {

                var iddonvi = Request["IdDonVi"];
                banner.IdDonVi = Convert.ToInt32(iddonvi);
                var f = Request.Files["file"];
                if (string.IsNullOrEmpty(banner.TieuDeChinh))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {

                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/GioiThieu/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/GioiThieu/" + FileName;
                }

                db.tbl_GioiThieu.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa thông tin giới thiệu ";
                a.Controller = "GioiThieu";
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
            tbl_GioiThieu banner = db.tbl_GioiThieu.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Banner đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }

            db.tbl_GioiThieu.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa thông tin giới thiệu " + banner.TieuDeChinh;
            a.Controller = "Banners";
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