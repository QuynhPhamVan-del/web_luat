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

    public class CamNhanController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page)
        {
            var list = db.tbl_CamNhan.ToList();
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
        [ValidateInput(false)]
        public JsonResult Create([Bind(Include = "Id,HoTen,NoiDung")] tbl_CamNhan BanGiamHieu)
        {
            try
            {

                db.tbl_CamNhan.Add(BanGiamHieu);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.Controller = "CamNhan";
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

        //// GET: Admin/BanGiamHieus/Edit/5
        public PartialViewResult Edit(int? id)
        {
            tbl_CamNhan BanGiamHieu = db.tbl_CamNhan.Find(id);

            return PartialView(BanGiamHieu);
        }
        [HttpPost]
        [ValidateInput(false)]

        public JsonResult Edit([Bind(Include = "Id,HoTen,NoiDung")] tbl_CamNhan BanGiamHieu)
        {
            try
            {
                db.tbl_CamNhan.AddOrUpdate(BanGiamHieu);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa cảm nhận ";
                a.Controller = "CamNhan";
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
                return Json(new { status = true, message = "Cảm nhận không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            tbl_CamNhan BanGiamHieu = db.tbl_CamNhan.Find(id);
            if (BanGiamHieu == null)
            {
                return Json(new { status = true, message = "Cảm nhận đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }

            db.tbl_CamNhan.Remove(BanGiamHieu);
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa cảm nhận ";
            a.Controller = "CamNhan";
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