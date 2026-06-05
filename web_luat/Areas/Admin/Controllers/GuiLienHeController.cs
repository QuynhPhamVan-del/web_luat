using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web_luat.Models;

namespace web_luat.Areas.Admin.Controllers
{
    [AdminAuthorize]

    public class GuiLienHeController : Controller
    {
        dbcontext db = new dbcontext();
        public ActionResult Index(int? page, string Keyword = "")
        {
            var list = db.GuiLienHes.AsQueryable();
            if (page == null) page = 1;
            var books = list.OrderByDescending(g => g.NgayGui);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.pageNumber = page;
            ViewBag.total = list.ToList().Count();
            ViewBag.key = Keyword;
            return View(books.ToPagedList(pageNumber, pageSize));
        }

    }
}