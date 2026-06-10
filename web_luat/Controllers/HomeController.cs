using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using web_luat.Models;


namespace web_luat.Controllers
{
    public class HomeController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index()
        {
            ViewData["banner"]=db.tbl_Banner.Where(g=>g.IsDelete==false).ToList();
            ViewBag.vechungtoi = db.tbl_GioiThieu.First();
            ViewData["dichvu"]= db.tbl_LienKetKhoa.Where(g => g.IsDelete == false).ToList();
            ViewData["nhansu"] = db.tbl_NhanSu.OrderBy(g => g.Id).Take(5).ToList();
            ViewData["thongso"] = db.tbl_ThongSo.ToList();
            ViewData["taisao"] = db.tbl_TaiSao.ToList();
            ViewData["doitac"] = db.tbl_DoiTac.Where(g => g.IsDelete == false).ToList();
            ViewData["camnhan"] = db.tbl_CamNhan.ToList();
            var lstId = db.tbl_Post_Category.Where(g => g.IdCategory == 3).Select(g => g.IdPost).ToList();
            ViewData["lstMoiNhat"] = db.tbl_Post.Where(g =>lstId.Contains(g.ID)).OrderByDescending(g => g.NgayPhatHanh).ToList();
            ViewBag.DuAn = db.tbl_ThuVienAnh.ToList();
            ViewBag.tvc = db.tbl_ThuVienAnh_Album.ToList();
            ViewData["lst"] = db.tbl_Post.Where(g => lstId.Contains(g.ID)).OrderByDescending(g => g.NgayPhatHanh).Take(3).ToList();
            return View();
        }

        public ActionResult Menu()
        {

            ViewData["menu"] = db.tbl_MenuPage.Where(g => g.IsDelete == false).OrderBy(g => g.STT).ToList();
            return View();
        }
        public ActionResult GioiThieu()
        {
            return View();
        }
        public ActionResult NhanSu()
        {
            ViewData["nhansu"] = db.tbl_NhanSu.ToList();
            return View();
        }
        public ActionResult LienHe()
        {
            return View();
        }

        public ActionResult GetCaptchaImage()
        {
            // Tạo chuỗi ngẫu nhiên gồm 5 ký tự (chữ và số)
            string text = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();

            // Lưu chuỗi này vào Session để đối chiếu lúc người dùng Submit
            Session["CaptchaText"] = text;

            // Tạo một bitmap để vẽ đồ họa hình ảnh mã
            using (Bitmap bitmap = new Bitmap(130, 45))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Tạo nền màu xám nhạt mịn màng hòa hợp với giao diện
                g.Clear(Color.FromArgb(241, 245, 249));

                // Vẽ các đường kẻ nhiễu sóng mắt ngăn chặn các bot quét ký tự (OCR)
                Random rand = new Random();
                using (Pen noisePen = new Pen(Color.FromArgb(226, 232, 240), 2))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        g.DrawLine(noisePen, rand.Next(0, 130), rand.Next(0, 45), rand.Next(0, 130), rand.Next(0, 45));
                    }
                }

                // Vẽ chuỗi ký tự Captcha lên hình ảnh với font chữ đậm nghệ thuật
                using (Font font = new Font("Arial", 20, FontStyle.Bold | FontStyle.Italic))
                using (Brush brush = new SolidBrush(Color.FromArgb(15, 23, 42))) // Màu Slate Dark trùng màu công ty bạn
                {
                    g.DrawString(text, font, brush, 15, 6);
                }

                // Xuất trực tiếp hình ảnh ra luồng dữ liệu View
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }

        [HttpPost]
        public JsonResult GuiLienHe(string fullname, string phone, string email, string service, string message, string captcha)
        {
            // 1. Kiểm tra dữ liệu đầu vào phía Server (Server-side Validation)
            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ các trường bắt buộc (*)." });
            }

            // 2. [Tùy chọn] Logic xác thực mã 'captcha' gửi lên với Google API
            // Nếu mã Captcha không hợp lệ, bạn trả về:
            // return Json(new { success = false, message = "Xác minh mã Captcha thất bại, vui lòng thử lại!" });

            try
            {
                GuiLienHe a = new GuiLienHe();
                a.HoTen = fullname;a.SDT = phone;a.Email = email;a.NgayGui=DateTime.Now;a.NoiDung = message;a.DichVu = service;
                db.GuiLienHes.Add(a);
                db.SaveChanges();
                return Json(new { success = true, message = "Gửi thông tin liên hệ thành công!" });
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi nếu quá trình lưu DB gặp trục trặc
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        public ActionResult TinTuc(int IdChuyenMuc = 0, string Key = "",int IdTag=0)
        {
            ViewBag.IdChuyenMucBanDau = IdChuyenMuc;
            ViewBag.KeyBanDau = Key;
            ViewBag.IdTag = IdTag;
            ViewData["chuyenmuc"] = db.tbl_ChuyenMuc.Where(g=>g.IsDelete==false).ToList();
            return View();
        }

        // ACTION CHUYÊN XỬ LÝ TRẢ VỀ JSON CHO ĐOẠN MÃ AJAX PHÂN TRANG (ĐÃ THÊM idChuyenMuc)
        [HttpGet]
        public JsonResult GetNewsJson(int page = 1, int pageSize = 4, string search = "", int idChuyenMuc = 0,int IdTag=0)
        {
            try
            {
                // 1. Khởi tạo truy vấn gốc (Lấy bài viết kèm ID chuyên mục đầu tiên để tránh trùng lặp)
                var baseQuery = db.tbl_Post.Select(p => new
                {
                    Post = p,
                    CategoryId = db.tbl_Post_Category.Where(pc => pc.IdPost == p.ID).Select(pc => pc.IdCategory).FirstOrDefault()
                })
                .Select(x => new
                {
                    x.Post,
                    x.CategoryId, // Giữ lại CategoryId để tiện tính toán hoặc hiển thị nếu cần
                    TenChuyenMuc = db.tbl_ChuyenMuc.Where(cm => cm.Id == x.CategoryId).Select(cm => cm.TenChuyenMuc).FirstOrDefault() ?? "Tin Tức"
                });

                // 2. Lọc theo Chuyên mục (SỬA LẠI Ở ĐÂY - CHỈ LỌC, KHÔNG GHI ĐÈ TRUY VẤN)
                if (idChuyenMuc > 0)
                {
                    // Lấy danh sách ID bài viết thuộc chuyên mục này
                    var lstId = db.tbl_Post_Category.Where(g => g.IdCategory == idChuyenMuc).Select(g => g.IdPost).ToList();

                    // Chỉ lọc những bài viết nằm trong danh sách ID trên
                    baseQuery = baseQuery.Where(x => lstId.Contains(x.Post.ID));

                    // Cập nhật lại tên chuyên mục chính xác theo bộ lọc cho đồng bộ dữ liệu
                    baseQuery = baseQuery.Select(x => new
                    {
                        x.Post,
                        x.CategoryId,
                        TenChuyenMuc = db.tbl_ChuyenMuc.Where(cm => cm.Id == idChuyenMuc).Select(cm => cm.TenChuyenMuc).FirstOrDefault() ?? "Tin Tức"
                    });
                }

                // 3. Nếu người dùng nhập từ khóa tìm kiếm, tiến hành lọc tiếp
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.Trim().ToLower();
                    baseQuery = baseQuery.Where(x => x.Post.TieuDe.ToLower().Contains(search) || x.Post.MoTaNgan.ToLower().Contains(search));
                }
                if (IdTag > 0)
                {
                    var lstIdPost = db.tbl_Post_Tag.Where(g => g.TagId == IdTag).Select(g => g.PostId).ToList();
                    baseQuery = baseQuery.Where(x => lstIdPost.Contains(x.Post.ID));
                }
                // 4. Tính toán tổng số lượng bài viết và tổng số trang sau khi ĐÃ QUA BỘ LỌC
                int totalRecords = baseQuery.Count();
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // 5. Thực hiện Phân trang (Skip / Take)
                var rawData = baseQuery.OrderByDescending(x => x.Post.NgayPhatHanh)
                                       .Skip((page - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToList();

                // 6. Định dạng cấu trúc dữ liệu trả về cho Client
                var formattedData = rawData.Select(x => new
                {
                    ID = x.Post.ID,
                    TieuDe = x.Post.TieuDe,
                    Image = x.Post.Image,
                    TomTat = x.Post.MoTaNgan ?? "",
                    NgayDang = x.Post.NgayPhatHanh.HasValue
                        ? x.Post.NgayPhatHanh.Value.ToString("'Tháng' MM 'ngày' dd, yyyy", new System.Globalization.CultureInfo("vi-VN"))
                        : "",
                    ChuyenMuc = x.TenChuyenMuc,
                    Url = $"/chi-tiet-tin/{convertChuoi.convertToUnSign2_2(x.Post.TieuDe)}_{x.Post.ID}.html"
                }).ToList();

                // 7. Trả khối kết quả JSON về cho AJAX
                return Json(new
                {
                    success = true,
                    data = formattedData,
                    totalPages = totalPages,
                    currentPage = page,
                    totalRecords = totalRecords
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ChiTietTin(int Id)
        {
            ViewData["lstMoiNhat"] = db.tbl_Post.OrderByDescending(g => g.NgayPhatHanh).Take(3).ToList();
            ViewData["lstIdTag"] = db.tbl_Post_Tag.Where(g => g.PostId == Id).ToList();
            ViewData["tag"] = db.tbl_Tag.ToList();
            ViewData["chuyenmuc"] = db.tbl_ChuyenMuc.Where(g => g.IsDelete == false).ToList();

            return View(db.tbl_Post.Find(Id));
        }

    }
}