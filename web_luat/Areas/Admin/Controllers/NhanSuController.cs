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
    public class NhanSuController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page,string KeyWord = "")
        {
            var list = db.tbl_NhanSu.AsQueryable();
            if (!string.IsNullOrEmpty(KeyWord))
            {
                list = list.Where(g => g.TenNhanSu.Contains(KeyWord));
            }
            ViewBag.key = KeyWord;
            if (page == null) page = 1;
            var books = list.OrderBy(g => g.Id).ToList();
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
        [ValidateInput(false)]
        public JsonResult Create([Bind(Include = "TenNhanSu,Image,ChucVu,GioiThieu")] tbl_NhanSu banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (f == null || string.IsNullOrEmpty(banner.TenNhanSu))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/NhanSu/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/NhanSu/" + FileName;
                }
                db.tbl_NhanSu.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới nhân sự " + banner.TenNhanSu;
                a.Controller = "NhanSu";
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
        public PartialViewResult Edit(int? id)
        {
            tbl_NhanSu banner = db.tbl_NhanSu.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit([Bind(Include = "Id,TenNhanSu,Image,ChucVu,GioiThieu")] tbl_NhanSu banner)
        {
            try
            {
                var f = Request.Files["file"];
                if (string.IsNullOrEmpty(banner.TenNhanSu))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);
                }
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/NhanSu/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/NhanSu/" + FileName;
                }
                db.tbl_NhanSu.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa nhân sự " + banner.TenNhanSu;
                a.Controller = "NhanSu";
                a.Action = "Edit";
                a.NgayThucHien = DateTime.Now;
                a.IdUser = Convert.ToInt32(Session["Id"].ToString());
                db.tbl_Log.AddOrUpdate(a);
                db.SaveChanges();
                return Json(new { status = true, message = "Sửa thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { status = true, message = "Banner không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_NhanSu banner = db.tbl_NhanSu.Find(id);
            if (banner == null)
            {
                return Json(new { status = true, message = "Banner đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            db.tbl_NhanSu.Remove(banner);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa nhân sự " + banner.TenNhanSu;
            a.Controller = "NhanSu";
            a.Action = "Delete";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
        }
    }
}