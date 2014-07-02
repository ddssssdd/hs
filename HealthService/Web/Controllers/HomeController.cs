using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "All tables";
            var list = new ArrayList();
            
            using (Db.DbHealthService db = new Db.DbHealthService())
            {
                db.executeSql("select * from tab", (dr) =>
                {
                    while (dr.Read())
                    {
                        list.Add(dr.GetValue(0));
                        System.Diagnostics.Debug.WriteLine(dr.GetValue(0));
                    }
                    
                    return false;
                });
            }
            ViewBag.tables = list;
            return View();
        }
        public ActionResult TableContent(String tablename)
        {
            ViewBag.tablename = tablename;

            ViewBag.rows = Db.DbHealthService.getTableToList(tablename);
            
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
    }
}
