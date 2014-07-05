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
        [HttpGet]
        public ActionResult Api()
        {
            var result = Db.DbHealthService.executeSqlToList(@"select * from view_app_inter order by row_id");
            ViewBag.items = result;
            return View();
        }
        [HttpPost]
        public ActionResult Api(String name,String url,String description,int item_id)
        {
            Db.Parameter[] parameters;
            if (item_id == 0)
            {
                parameters = new[]{ 
                new Db.Parameter{name="v_jkm",value=name},
                new Db.Parameter{name="v_url",value=url},
                new Db.Parameter{name="v_miaosh",value=description},
                new Db.Parameter{name="v_rt",value=0,type=2,direction=2}
                };
            }
            else
            {
                parameters = new[]{ 
                new Db.Parameter{name="v_jkm",value=name},
                new Db.Parameter{name="v_url",value=url},
                new Db.Parameter{name="v_miaosh",value=description},
                new Db.Parameter{name="v_id",value=item_id,type=2},
                new Db.Parameter{name="v_rt",value=0,type=2,direction=2}
                };
            }

            int id = Db.DbHealthService.ExecuteSP("pack_jiekou.InsertJkInfo", parameters, "v_rt");
            var result = Db.DbHealthService.executeSqlToList(@"select * from view_app_inter order by row_id");
            ViewBag.items = result;
            return View();
        }
    }
}
