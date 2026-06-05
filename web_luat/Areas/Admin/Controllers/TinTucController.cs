using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using web_luat.Models;

namespace web_luat.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class TinTucController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, string Keyword = "", int IdChuyenMuc = 0, bool isTrash = false, bool isNoiBat = false)
        {
            var lstCM = db.tbl_ChuyenMuc.Where(g => g.IsDelete == false).ToList();
            ViewData["loaitin"] = lstCM;
            var list = db.tbl_Post.Where(g => g.IsDelete == isTrash && g.IsActive == true).OrderByDescending(g => g.NgayPhatHanh).AsQueryable();
            if (IdChuyenMuc > 0)
            {
                list = list.Where(g => g.IdChuyenMuc == IdChuyenMuc);
            }
            if (!string.IsNullOrEmpty(Keyword))
            {
                list = list.Where(g => g.TieuDe.ToLower().Contains(Keyword.ToLower()));
            }
            if (isNoiBat == true)
            {
                list = db.tbl_Post.Where(g => g.IsNoiBat == true);
            }
            if (page == null) page = 1;
            IOrderedQueryable<tbl_Post> books;

            if (isNoiBat)
            {
                books = list
                    .OrderBy(g => g.STTUuTien == null)
                    .ThenBy(g => g.STTUuTien)
                    .ThenByDescending(g => g.NgayPhatHanh);
            }
            else
            {
                books = list
                    .OrderByDescending(g => g.NgayPhatHanh);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.pageNumber = page;
            ViewBag.total = list.Count();
            ViewBag.IdChuyenMuc = IdChuyenMuc;
            ViewBag.Keyword = Keyword;
            ViewBag.isNoiBat = isNoiBat == true ? 1 : 0;
            return View(books.ToPagedList(pageNumber, pageSize));
        }
        public JsonResult GetTags(string q)
        {
            q = (q ?? "").Trim().ToLower();

            var tags = db.tbl_Tag
                .Where(t => t.TenTag.ToLower().Contains(q))
                .Select(t => new
                {
                    id = t.TenTag,
                    text = t.TenTag
                })
                .Take(20)
                .ToList();

            return Json(tags, JsonRequestBehavior.AllowGet);
        }
        private void SaveTags(int postId, string[] tags)
        {
            if (tags == null || tags.Length == 0) return;

            var tagNames = tags
                .Select(t => t.Trim().ToLower())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .ToList();

            var existTags = db.tbl_Tag
                .Where(t => tagNames.Contains(t.TenTag))
                .ToList();

            var existTagNames = existTags.Select(t => t.TenTag).ToList();
            var newTags = tagNames
                .Where(t => !existTagNames.Contains(t))
                .Select(t => new tbl_Tag
                {
                    TenTag = t,
                    Slug = GenerateSlug(t)
                }).ToList();

            // 👉 THAY THẾ CHỖ NÀY: Dùng foreach thay cho AddRange của tbl_Tag
            if (newTags.Any())
            {
                foreach (var tag in newTags)
                {
                    db.tbl_Tag.Add(tag);
                }
                db.SaveChanges();
            }

            var allTags = db.tbl_Tag
                .Where(t => tagNames.Contains(t.TenTag))
                .ToList();

            var existPostTags = db.tbl_Post_Tag
                .Where(x => x.PostId == postId)
                .Select(x => x.TagId)
                .ToList();

            var postTags = allTags
                .Where(t => !existPostTags.Contains(t.Id))
                .Select(t => new tbl_Post_Tag
                {
                    PostId = postId,
                    TagId = t.Id
                }).ToList();

            // 👉 THAY THẾ CHỖ NÀY: Dùng foreach thay cho AddRange của tbl_Post_Tag
            if (postTags.Any())
            {
                foreach (var postTag in postTags)
                {
                    db.tbl_Post_Tag.Add(postTag);
                }
                db.SaveChanges();
            }
        }
        private string GenerateSlug(string text)
        {
            text = text.ToLower().Trim();

            text = text.Replace("á", "a").Replace("à", "a").Replace("ả", "a")
                       .Replace("ã", "a").Replace("ạ", "a");

            text = text.Replace("đ", "d");

            text = System.Text.RegularExpressions.Regex
                .Replace(text, @"[^a-z0-9\s-]", "");

            text = text.Replace(" ", "-");

            return text;
        }
        public JsonResult GetTagsByPost(int postId)
        {
            var tags = (from pt in db.tbl_Post_Tag
                        join t in db.tbl_Tag on pt.TagId equals t.Id
                        where pt.PostId == postId
                        select new
                        {
                            id = t.TenTag,
                            text = t.TenTag
                        }).ToList();

            return Json(tags, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Create()
        {
            ViewData["loaitin"] = db.tbl_ChuyenMuc.Where(g => g.IsDelete == false).ToList();
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Create([Bind(Include = "Id,TieuDe,MoTaNgan,STTUuTien,Image,NoiDung,IsActive,View,IsDelete,TacGia,IsLanguage,IdChuyenMuc,NgayPhatHanh,IdDonVi,Nguon,Link,IsTab")] tbl_Post banner, string[] Tags)
        {
            try
            {

                var f = Request.Files["file"];
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/TinTuc/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/TinTuc/" + FileName;
                }
                banner.IdUser = Convert.ToInt32(Session["Id"].ToString());
                var dem = db.QuyTrinhDuyets.Count();

                banner.NgayPhatHanh = DateTime.Now;
                banner.View = 0;
                banner.TacGia = Session["HoTen"].ToString();
                banner.IsDelete = false;
                banner.IsActive = false; // chưa public
                if (dem == 0)
                {
                    banner.IsActive = true;
                }
                db.tbl_Post.Add(banner);
                db.SaveChanges();
                var stt = Convert.ToInt32(Request["stt"]);
                for (int i = 1; i <= stt; i++)
                {
                    var IdChuyenMuc = 0;
                    IdChuyenMuc = Convert.ToInt32(Request.Form["IdChuyenMuc_" + i]);

                    if (IdChuyenMuc > 0 )
                    {
                        tbl_Post_Category m = new tbl_Post_Category();
                        m.IdPost = banner.ID;
                        m.IdCategory = IdChuyenMuc;
                        db.tbl_Post_Category.Add(m);
                        db.SaveChanges();
                    }
                }
                SaveTags(banner.ID, Tags);
                tbl_Log a = new tbl_Log();
                a.MoTa = "Thêm mới tin tức: " + banner.TieuDe;
                a.Controller = "TinTuc";
                a.Action = "Create";
                a.NgayThucHien = DateTime.Now;
                a.IdUser = Convert.ToInt32(Session["Id"].ToString());
                db.tbl_Log.Add(a);
                db.SaveChanges();
                return Json(new { status = true, message = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Add(int stt)
        {
            ViewData["loaitin"] = db.tbl_ChuyenMuc.Where(g => g.IsDelete == false).ToList();
            ViewBag.stt = stt;
            return View();
        }
        public PartialViewResult AddChuyenMuc(int? id)
        {
            tbl_Post banner = db.tbl_Post.Find(id);
            ViewData["loaitin"] = db.tbl_ChuyenMuc.Where(g => g.IsDelete == false).ToList();
            return PartialView(banner);
        }

        public PartialViewResult Edit(int? id)
        {

            tbl_Post banner = db.tbl_Post.Find(id);
            ViewData["loaitin"] = db.tbl_ChuyenMuc.Where(g => g.IsDelete == false).ToList();
            var lst = db.tbl_Post_Category.Where(g => g.IdPost == id).ToList();
            ViewData["lst"] = lst;
            return PartialView(banner);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit([Bind(Include = "Id,TieuDe,MoTaNgan,STTUuTien,Image,NoiDung,IsActive,View,IsDelete,TacGia,IdChuyenMuc,NgayPhatHanh,IdDonVi,Nguon,BuocDuyet,TrangThai,IdUser,Link,IsTab")] tbl_Post banner, string[] Tags)
        {
            try
            {

                var f = Request.Files["file"];
                if (f != null && f.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~" + banner.Image);
                    if (System.IO.File.Exists(folderPath))
                    {
                        System.IO.File.Delete(folderPath);
                    }
                    string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + f.FileName);
                    string UploadPath = Server.MapPath("~/Upload/TinTuc/" + FileName);
                    f.SaveAs(UploadPath);
                    banner.Image = "/Upload/TinTuc/" + FileName;
                }
                var lst = db.tbl_Post_Category.Where(g => g.IdPost == banner.ID).ToList();
                foreach(var item in lst)
                {
                    db.tbl_Post_Category.Remove(item);

                }
                db.SaveChanges();


                var stt = Convert.ToInt32(Request["stt"]);
                for (int i = 1; i <= stt; i++)
                {
                    var IdChuyenMuc = 0;
                    IdChuyenMuc = Convert.ToInt32(Request.Form["IdChuyenMuc_" + i]);
       

                    if ( IdChuyenMuc > 0 )
                    {
                        tbl_Post_Category m = new tbl_Post_Category();
                        m.IdPost = banner.ID;
                        m.IdCategory = IdChuyenMuc;

                        db.tbl_Post_Category.Add(m);
                        db.SaveChanges();
                    }
                }
                banner.IsActive = true;
                banner.IsDelete = false;
                db.tbl_Post.AddOrUpdate(banner);
                db.SaveChanges();

                var IdMenu = Convert.ToInt32(Request["IdMenu"]);
                if (IdMenu > 0)
                {
                    var check = db.tbl_MenuPage.Where(g => g.IdBaiViet == banner.ID);
                    if (check.Count() > 0)
                    {
                        if (check.First().Id != IdMenu)
                        {
                            var m = db.tbl_MenuPage.FirstOrDefault(g => g.IdBaiViet == banner.ID);
                            m.IdBaiViet = null;
                            m.Link = null;
                            db.tbl_MenuPage.AddOrUpdate(m);
                            db.SaveChanges();
                            var l = db.tbl_MenuPage.FirstOrDefault(g => g.Id == IdMenu);
                            l.IdBaiViet = banner.ID;
                            l.Link = "/chi-tiet-tin/" + convertChuoi.convertToUnSign2_2(l.TenDonVi).Replace(" ", "_") + "_" + banner.ID + ".html";
                            db.tbl_MenuPage.AddOrUpdate(l);
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        var l = db.tbl_MenuPage.FirstOrDefault(g => g.Id == IdMenu);
                        l.IdBaiViet = banner.ID;
                        l.Link = "/chi-tiet-tin/" + convertChuoi.convertToUnSign2_2(l.TenDonVi).Replace(" ", "_") + "_" + banner.ID + ".html";
                        db.tbl_MenuPage.AddOrUpdate(l);
                        db.SaveChanges();
                    }

                }



                var oldTags = db.tbl_Post_Tag.Where(x => x.PostId == banner.ID);
                foreach(var item in oldTags)
                {
                    db.tbl_Post_Tag.Remove(item);

                }
                db.SaveChanges();
                tbl_Log a = new tbl_Log();
                a.MoTa = "Sửa tin tức: " + banner.TieuDe;
                a.Controller = "TinTuc";
                a.Action = "Edit";
                a.NgayThucHien = DateTime.Now;
                a.IdUser = Convert.ToInt32(Session["Id"].ToString());
                db.tbl_Log.Add(a);
                // 👉 LƯU TAG MỚI
                SaveTags(banner.ID, Tags);


                return Json(new { status = true, message = "Cập nhật dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = true, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GuiDuyet(int id)
        {
            var post = db.tbl_Post.Find(id);
            if (post == null)
                return Json(new { status = false, message = "Không tồn tại" });

            post.TrangThai = 1; // chờ duyệt
            tbl_Log a = new tbl_Log();
            a.MoTa = "Gửi duyệt tin tức: " + post.TieuDe;
            a.Controller = "TinTuc";
            a.Action = "GuiDuyet";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();


            return Json(new { status = true, message = "Đã gửi duyệt" });
        }
        public ActionResult ChoDuyet(int? page, string Keyword = "", int IdChuyenMuc = 0, bool isTrash = false)
        {
            if (Session["IdDV"] == null || Session["Id"] == null)
                return RedirectToAction("Login", "Account");

            int IdDV = Convert.ToInt32(Session["IdDV"]);
            int IdUser = Convert.ToInt32(Session["Id"]);

            // Lấy danh sách chuyên mục theo đơn vị
            var lstId = db.tbl_DonViChuyenMuc
                          .Where(x => x.IdDonVi == IdDV)
                          .Select(x => x.IdChuyenMuc);

            var lstCM = db.tbl_ChuyenMuc
                          .Where(x => x.IsDelete == false && (IdDV == 0 || lstId.Contains(x.Id)))
                          .ToList();

            ViewData["loaitin"] = lstCM;

            // Lấy nhóm quyền
            var nhomQuyen = db.tbl_User_NhomQuyen
                              .FirstOrDefault(x => x.IdUser == IdUser);

            if (nhomQuyen == null)
                return View();

            var quyTrinh = db.QuyTrinhDuyets
                             .FirstOrDefault(x => x.IdNhomQuyen == nhomQuyen.IdNhomQuyen);

            if (quyTrinh == null)
                return View();

            int? buoc = quyTrinh.STT;

            // Query chính (GIỮ IQueryable)
            var query = db.tbl_Post.Where(g =>
                            g.IsDelete == false &&
                            g.TrangThai == 1 &&
                            g.BuocDuyet == buoc
                        );

            // Filter theo chuyên mục
            if (IdChuyenMuc > 0)
            {

                query = query.Where(g => g.IdChuyenMuc == IdChuyenMuc);
            }

            // Filter theo keyword
            if (!string.IsNullOrEmpty(Keyword))
            {
                Keyword = Keyword.ToLower();
                query = query.Where(g => g.TieuDe.ToLower().Contains(Keyword));
            }

            // Paging
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var result = query.OrderByDescending(g => g.ID);

            ViewBag.pageNumber = pageNumber;
            ViewBag.total = result.Count();
            ViewBag.IdChuyenMuc = IdChuyenMuc;
            ViewBag.Keyword = Keyword;

            return View(result.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public JsonResult Duyet(int id, int action, string ghiChu)
        {
            var post = db.tbl_Post.Find(id);
            if (post == null)
                return Json(new { status = false });

            int buoc = post.BuocDuyet ?? 1;

            int maxStep = db.QuyTrinhDuyets.Max(x => x.STT) ?? 1;

            if (action == 1) // DUYỆT
            {
                if (buoc >= maxStep)
                {
                    post.TrangThai = 3; // đã duyệt
                    post.IsActive = true;
                    post.NgayPhatHanh = DateTime.Now;

                }
                else
                {
                    post.BuocDuyet = buoc + 1;
                }
            }
            else // TỪ CHỐI
            {
                post.TrangThai = 2; // bị từ chối
            }

            // log
            db.tbl_Post_DuyetLog.Add(new tbl_Post_DuyetLog
            {
                PostId = post.ID,
                BuocDuyet = buoc,
                NguoiXuLy = Convert.ToInt32(Session["UserId"]),
                TrangThai = action,
                GhiChu = ghiChu,
                NgayXuLy = DateTime.Now
            });
            tbl_Log a = new tbl_Log();
            a.MoTa = "Duyệt tin tức: " + post.TieuDe;
            a.Controller = "TinTuc";
            a.Action = "Duyet";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);
            db.SaveChanges();


            return Json(new { status = true });
        }
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { status = true, message = "Tin tức không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            if (db.tbl_MenuPage.Count(g => g.IdBaiViet == id) > 0)
            {
                var m = db.tbl_MenuPage.FirstOrDefault(g => g.IdBaiViet == id);
                m.Link = null;
                m.IdBaiViet = 0;
                db.tbl_MenuPage.AddOrUpdate(m);
                db.SaveChanges();
            }

            tbl_Post banner = db.tbl_Post.Find(id);
            var menu = db.tbl_MenuPage.Count(g => g.IdBaiViet == id);
            if (menu > 0)
            {
                var menu1 = db.tbl_MenuPage.FirstOrDefault(g => g.IdBaiViet == id);
                menu1.IdBaiViet = 0;
                db.tbl_MenuPage.AddOrUpdate(menu1);
                db.SaveChanges();
            }
            if (banner == null)
            {
                return Json(new { status = true, message = "Tin tức đã bị xóa." }, JsonRequestBehavior.AllowGet);
            }
            db.tbl_Post.Remove(banner);
            db.SaveChanges();
            tbl_Log a = new tbl_Log();
            a.MoTa = "Xóa tin tức: " + banner.TieuDe;
            a.Controller = "TinTuc";
            a.Action = "Delete";
            a.NgayThucHien = DateTime.Now;
            a.IdUser = Convert.ToInt32(Session["Id"].ToString());
            db.tbl_Log.Add(a);


            return Json(new { status = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UploadPdf(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return Json(new { uploaded = false, error = "Không có file" });

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".pdf")
                return Json(new { uploaded = false, error = "Chỉ cho phép PDF" });
            var url = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + file.FileName);
                string UploadPath = Server.MapPath("~/Upload/TinTuc/" + FileName);
                file.SaveAs(UploadPath);
                url = "/Upload/TinTuc/" + FileName;
            }

            return Json(new { uploaded = true, url = url });
        }
        [HttpPost]
        public JsonResult UploadVideo(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return Json(new { uploaded = false, error = "Không có file" });
            }

            var ext = Path.GetExtension(file.FileName).ToLower();

            if (ext != ".mp4" && ext != ".webm" && ext != ".ogg")
            {
                return Json(new { uploaded = false, error = "Chỉ cho phép video mp4/webm/ogg" });
            }
            var url = "";
            if (file != null && file.ContentLength > 0)
            {
                string FileName = System.IO.Path.GetFileName(DateTime.Now.ToString("ddMMyyyyhhmmss") + file.FileName);
                string UploadPath = Server.MapPath("~/Upload/TinTuc/" + FileName);
                file.SaveAs(UploadPath);
                url = "/Upload/TinTuc/" + FileName;
            }

            return Json(new { uploaded = true, url = url });
        }

        public ActionResult BiTuChoi(int? page, string Keyword = "", int IdChuyenMuc = 0, bool isTrash = false)
        {
            var IdUsser = Convert.ToInt32(Session["Id"].ToString());
            ViewData["loaitin"] = db.tbl_ChuyenMuc.Where(g => g.IsDelete == false).ToList();
            var list = db.tbl_Post
                .Where(g => g.IsDelete == isTrash
                         && g.TrangThai == 2 && g.IdUser == IdUsser) // bị từ chối
                .OrderByDescending(g => g.ID)
                .AsQueryable();

            if (IdChuyenMuc > 0)
            {
                list = list.Where(g => g.IdChuyenMuc == IdChuyenMuc);
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                list = list.Where(g => g.TieuDe.ToLower().Contains(Keyword.ToLower()));
            }


            if (page == null) page = 1;

            var books = list.OrderByDescending(g => g.ID);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.pageNumber = page;
            ViewBag.total = list.Count();
            ViewBag.IdChuyenMuc = IdChuyenMuc;
            ViewBag.Keyword = Keyword;

            return View(books.ToPagedList(pageNumber, pageSize));
        }


    }
}