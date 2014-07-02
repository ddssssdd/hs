using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(String txt_sql) 
        {
            txt_sql = Request["txt_sql"];
            if (!String.IsNullOrEmpty(txt_sql))
            {
                ViewBag.sql = txt_sql;
                ViewBag.rows = Db.DbHealthService.executeSqlToList(txt_sql);
            }
            return View();
        }

    }
}
