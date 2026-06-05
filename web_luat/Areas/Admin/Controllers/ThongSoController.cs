
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
    public class ThongSoController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, string Keyword = "")
        {
            var list = db.tbl_ThongSo.AsQueryable();
            if (!string.IsNullOrEmpty(Keyword))
            {
                list = list.Where(g => g.TieuDe.Contains(Keyword));
            }
            if (page == null) page = 1;
            var books = list.OrderBy(g => g.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.pageNumber = page;
            ViewBag.total = list.ToList().Count();
            ViewBag.key = Keyword;
            return View(books.ToPagedList(pageNumber, pageSize));
        }

        public PartialViewResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult Create([Bind(Include = "Id,TieuDe,ThongSo,KiHieu")] tbl_ThongSo banner)
        {
            try
            {
                if (string.IsNullOrEmpty(banner.TieuDe))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);

                }
                db.tbl_ThongSo.Add(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới thông số   " + banner.TieuDe;
                a.Controller = "ThongSo";
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
            tbl_ThongSo banner = db.tbl_ThongSo.Find(id);
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id,TieuDe,ThongSo,KiHieu")] tbl_ThongSo banner)
        {
            try
            {
                if (string.IsNullOrEmpty(banner.TieuDe))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);

                }
                db.tbl_ThongSo.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa thông số  " + banner.TieuDe;
                a.Controller = "ThongSo";
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
                return Json(new { status = true, message = "Chuyên mục không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_ThongSo banner = db.tbl_ThongSo.Find(id);
            db.tbl_ThongSo.Remove(banner);
            db.SaveChanges();
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa thông số " + banner.TieuDe;
            a.Controller = "ThongSo";
            a.Action = "Delete";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
        }
    }
}