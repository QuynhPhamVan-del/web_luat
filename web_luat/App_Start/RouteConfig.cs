using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace web_luat
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
               name: "ChiTietTin",
               url: "chi-tiet-tin/{alias}_{Id}.html",
               defaults: new { controller = "Home", action = "ChiTietTin", Id = UrlParameter.Optional }
               );
            routes.MapRoute(
         name: "Tintuc",
         url: "danh-sach-tin-tuc.html",
         defaults: new { controller = "Home", action = "TinTuc" }
         );
            routes.MapRoute(
  name: "GioiThieu",
  url: "gioi-thieu.html",
  defaults: new { controller = "Home", action = "GioiThieu" }
  );
            routes.MapRoute(
name: "NhanSu",
url: "nhan-su.html",
defaults: new { controller = "Home", action = "NhanSu" }
);
            routes.MapRoute(
name: "LienHe",
url: "lien-he.html",
defaults: new { controller = "Home", action = "LienHe" }
);
            routes.MapRoute(
name: "DSTintuc",
url: "danh-sach-tin/{alias}_{IdChuyenMuc}.html",
defaults: new { controller = "Home", action = "TinTuc", IdChuyenMuc = UrlParameter.Optional }
);
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
