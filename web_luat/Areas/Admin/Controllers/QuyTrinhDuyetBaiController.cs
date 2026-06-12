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
    public class QuyTrinhDuyetBaiController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, string Keyword = "")
        {
            ViewData["nhomquyen"] = db.tbl_NhomQuyen.Where(g => g.IsDelete == false).ToList();
            var list = db.QuyTrinhDuyets.AsQueryable().OrderBy(g => g.STT);
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
            ViewData["nhomquyen"] = db.tbl_NhomQuyen.Where(g => g.IsDelete == false).ToList();
            return PartialView();
        }
        [HttpPost]
        public JsonResult Create([Bind(Include = "Id,STT,TenBuoc")] QuyTrinhDuyet banner)
        {
            try
            {
                if (string.IsNullOrEmpty(banner.TenBuoc))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);

                }
                var nhomQuyen = Request["IdNhomQuyen"];
                foreach (var item in nhomQuyen.Split(',').ToList())
                {
                    QuyTrinhDuyet b = new QuyTrinhDuyet();
                    b.STT = banner.STT;
                    b.TenBuoc = banner.TenBuoc;
                    b.IdNhomQuyen = Convert.ToInt32(item);
                    db.QuyTrinhDuyets.Add(b);
                }
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới quy trình duyệt";
                a.Controller = "QuyTrinhDuyet";
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
            ViewData["nhomquyen"] = db.tbl_NhomQuyen.Where(g => g.IsDelete == false).ToList();
            QuyTrinhDuyet banner = db.QuyTrinhDuyets.FirstOrDefault(g => g.STT == id);
            List<int?> a = new List<int?>();
            foreach (var item in db.QuyTrinhDuyets.Where(g => g.STT == id).ToList())
            {
                a.Add(item.IdNhomQuyen);
            }
            ViewData["int"] = a;
            return PartialView(banner);
        }
        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id,STT,IdNhomQuyen,TenBuoc")] QuyTrinhDuyet banner)
        {
            try
            {
                if (string.IsNullOrEmpty(banner.TenBuoc))
                {
                    return Json(new { status = false, message = "Không được để trống các dữ liệu bắt buộc." }, JsonRequestBehavior.AllowGet);

                }
                var lst = db.QuyTrinhDuyets.ToList();
                foreach(var item in lst)
                {
                    db.QuyTrinhDuyets.Remove(item);

                }
                db.SaveChanges();
                var nhomQuyen = Request["IdNhomQuyen"];
                foreach (var item in nhomQuyen.Split(',').ToList())
                {
                    QuyTrinhDuyet b = new QuyTrinhDuyet();
                    b.STT = banner.STT;
                    b.TenBuoc = banner.TenBuoc;
                    b.IdNhomQuyen = Convert.ToInt32(item);

                    db.QuyTrinhDuyets.Add(b);
                }
                db.SaveChanges();
                db.QuyTrinhDuyets.AddOrUpdate(banner);
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa quy trình duyệt";
                a.Controller = "QuyTrinhDuyet";
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
            var banner = db.QuyTrinhDuyets.Where(g => g.STT == id);
            foreach (var item in banner)
            {
                db.QuyTrinhDuyets.Remove(item);

            }
            db.SaveChanges();
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa quy trình duyệt ";
            a.Controller = "QuyTrinhDuyet";
            a.Action = "Delete";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();
            return Json(new { status = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
        }
    }
}