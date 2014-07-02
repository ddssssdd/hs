using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ServiceController : Controller
    {
        #region User Login
        public JsonResult login(String username,String password)
        {
            String sql = String.Format(@"select md5('{1}') as curPass,a.* from view_huiyuan a where a.dianhua='{0}'",username,password);
            
            var results = Db.DbHealthService.ExecuteSql(sql);
            if (results.Count > 0)
            {
                Dictionary<String, Object> firstone = results[0] as Dictionary<String, Object>;
                if (firstone["USER_PASSWORD"].ToString().Equals(firstone["CURPASS"].ToString()))
                {
                    var result_success = new { status = true, result = results[0] };
                    return Json(result_success, JsonRequestBehavior.AllowGet);
                }
                
                
            }
            var result_failure = new { status = false, message = "Username or password is incorrect." };
            return Json(result_failure, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Category Datas
        //宣传图片
        public JsonResult ad()
        {
            String sql = "select * from view_xuanch_pic";
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //运动分类
        public JsonResult sport_category()
        {
            String sql = "select * from view_yund_fl";
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //运动项目
        public JsonResult sport(String id,String no)
        {
            String sql = String.Format(@"select a.*,pack_huiyuan.IsFavSport('{1}',yund_bianh) as isFavSport  from view_yund a where fl_bianh='{0}'", id,no);
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //我喜欢的运动
        public JsonResult my_sports(String no)
        {
            String sql = "select a.* from view_yund a where pack_huiyuan.IsFavSport(:no,yund_bianh) =1";
            Db.Parameter[] parameters = { new Db.Parameter { name = "no", value = no } };
            var results = Db.DbHealthService.ExecuteSqlWithParams(sql, parameters);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }

        //食物分类
        public JsonResult food_category()
        {
            String sql = "select * from view_shiwu_fl";
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //食物
        public JsonResult food(String id,String no)
        {
            String sql = String.Format(@"select a.*,pack_huiyuan.IsFavFood('{1}',shiw_bianh) as isFavFood from view_shiwu a where fl_bianh='{0}'", id,no);
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //我收藏的食物
        public JsonResult my_foods(String no)
        {
            String sql = "select a.* from view_shiwu a where pack_huiyuan.IsFavFood(:no,shiw_bianh) =1";
            Db.Parameter[] parameters = { new Db.Parameter { name = "no", value = no } };
            var results = Db.DbHealthService.ExecuteSqlWithParams(sql, parameters);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }

        //药品分类
        public JsonResult medicine_category()
        {
            String sql = "select * from view_yaopin_fenl";
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //药品信息
        public JsonResult medicine(String id)
        {
            String sql = String.Format(@"select * from view_yaopin where fl_bianhao='{0}'", id);
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Physical Examination
        //体检

        public JsonResult physical_examination(String no) 
        {
            String sql = String.Format(@"select * from view_tijian where bianhao='{0}'", no);
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //体检项目
        public JsonResult pe_items(String pe_id)
        {
            String sql = String.Format(@"select * from view_tijian_xm where main_id={0}", pe_id);
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //体检项目结果
        public JsonResult pe_results(String pe_id)
        {
            String sql = String.Format(@"select * from view_tijian_jieg where main_id={0}", pe_id);
            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Add/Remove Favorite 
        //收藏/取消食物
        public JsonResult favorite_food(String no, String food_id,int favor=1) {
            /*
            Db.Parameter[] list = new Db.Parameter[3];
            list[0] = new Db.Parameter { name = "v_bh", value = no};
            list[1] = new Db.Parameter { name = "v_swbh", value = food_id};
            list[2] = new Db.Parameter { name = "v_rt", value = 0, type = 2, direction = 2 };
            int result = Db.DbHealthService.ExecuteSP(favor == 1 ? "pack_huiyuan.AddFavFood" : "pack_huiyuan.DelFavFood", list);
             */
            Db.Parameter[] list ={ 
                                     new Db.Parameter { name = "v_bh", value = no},
                                     new Db.Parameter { name = "v_swbh", value = food_id},
                                     new Db.Parameter { name = "v_rt", value = 0, type = 2, direction = 2 }
                                };
            int result = Db.DbHealthService.ExecuteSP(favor == 1 ? "pack_huiyuan.AddFavFood" : "pack_huiyuan.DelFavFood",list);
            return Json(new { status=true}, JsonRequestBehavior.AllowGet);
        }
        //收藏/取消运动
        public JsonResult favorite_sport(String no, String sport_id, int favor = 1)
        {
           
            Db.Parameter[] list ={ 
                                     new Db.Parameter { name = "v_bh", value = no},
                                     new Db.Parameter { name = "v_ydbh", value = sport_id},
                                     new Db.Parameter { name = "v_rt", value = 0, type = 2, direction = 2 }
                                };
            int result = Db.DbHealthService.ExecuteSP(favor == 1 ? "pack_huiyuan.AddFavSport" : "pack_huiyuan.DelFavSport", list);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Test
        public JsonResult test()
        { 
            Db.Parameter[] list = {new Db.Parameter{name="bianhao",value="140601"}};
            var results =Db.DbHealthService.ExecuteSqlWithParams("select * from view_tijian where bianhao=:bianhao",list);
            return Json(new {status=true,list=results},JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
