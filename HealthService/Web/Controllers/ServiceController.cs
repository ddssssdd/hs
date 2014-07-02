﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        //体检结果
        public JsonResult pe_details(String pe_id)
        {
            String sql = String.Format(@"select * from view_tijian_xm where main_id={0}", pe_id);
            var results = Db.DbHealthService.ExecuteSql(sql);
            sql = String.Format(@"select * from view_tijian_jieg where main_id={0}", pe_id);
            var results2 = Db.DbHealthService.ExecuteSql(sql);
            foreach (Dictionary<String,Object> item in results)
            {
                List<Dictionary<String, Object>> items = new List<Dictionary<string, object>>();
                String main_id = item["MAIN_ID"].ToString();
                foreach (Dictionary<String,Object>  r in results2) { 
                    if (main_id.Equals(r["MAIN_ID"].ToString()))
                    {
                        items.Add(r);
                    }
                }
                item["PE_RESULTS"]=items;
            }
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
        #region Information
        //消息列表
        public JsonResult information(String no)
        {
            String sql = @"select * from view_huiy_xiaox where bianhao=:no";
            Db.Parameter[] list = { new Db.Parameter { name="no",value=no} };
            var results = Db.DbHealthService.ExecuteSqlWithParams(sql, list);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //阅读消息
        public JsonResult read_information(int id)
        {
            Db.Parameter[] list ={ 
                                     new Db.Parameter { name = "v_id", value = id, type = 2, direction = 1 }
                                };
            int result = Db.DbHealthService.ExecuteSP("pack_huiyuan1.ReadMess", list);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Person center
        public JsonResult person_center(String no)
        {
            
            String sql_infor = @"select * from view_huiy_xiaox where bianhao=:no";
            Db.Parameter[] list_infor = { new Db.Parameter { name = "no", value = no } };
            var results_infor = Db.DbHealthService.ExecuteSqlWithParams(sql_infor, list_infor);
            String sql_score = @"select * from view_huiy_mub where bianhao=:no";
            Db.Parameter[] list_score = { new Db.Parameter { name = "no", value = no } };
            var results_score = Db.DbHealthService.ExecuteSqlWithParams(sql_score, list_score);
            return Json(new { status = true, result = new { score=results_score,information=results_infor} }, JsonRequestBehavior.AllowGet);
        }
        //监控数据来源表
        public JsonResult ph_check_items(String no)
        {
            String sql = @"select * from view_jiankong where bianhao=:no order by shangbao_riqi desc";
            Db.Parameter[] list = { new Db.Parameter { name = "no", value = no } };
            var results = Db.DbHealthService.ExecuteSqlWithParams(sql, list);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //监控数据定义
        public JsonResult ph_define_items()
        {
            String sql = @"select * from view_jiank_xm";

            var results = Db.DbHealthService.ExecuteSql(sql);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //会员监控目标值
        public JsonResult ph_targets(String no)
        {
            String sql = @"select * from view_ huiy_mub where bianhao=:no";
            Db.Parameter[] list = { new Db.Parameter { name = "no", value = no } };
            var results = Db.DbHealthService.ExecuteSqlWithParams(sql, list);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //健康数据上报
        public JsonResult ph_add(String no,DateTime monitor_date,String item,String value)
        
        {
            Db.Parameter[] parameters = {
                                        new Db.Parameter{name="v_bh",value=no},
                                        new Db.Parameter{name="v_jkd",value=no,type=3},
                                        new Db.Parameter{name="v_jkx",value=no},
                                        new Db.Parameter{name="v_jkv",value=no},
                                        new Db.Parameter{name="v_rt",value=0,type=2,direction=2},
                                        };
            int result = Db.DbHealthService.ExecuteSP("pack_huiyuan.UpJiankong", parameters, "v_rt");
            return Json(new { status = true}, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Question
        public JsonResult my_questions(String no)
        {
            String sql = @"select * from view_huiyuan_zixun where hy_bianhao=:no";
            Db.Parameter[] list = { new Db.Parameter { name = "no", value = no } };
            var results = Db.DbHealthService.ExecuteSqlWithParams(sql, list);
            return Json(new { status = true, result = results }, JsonRequestBehavior.AllowGet);
        }
        //咨询内容提交
        public JsonResult add_question(String no,String type,String content)
        {
            Db.Parameter[] parameters= {
                             new Db.Parameter{name="v_bh",value=no},
                             new Db.Parameter{name="v_lx",value=type},
                             new Db.Parameter{name="v_ms",value=content},
                             new Db.Parameter{name="v_rt",value=0,type=2,direction=2}
                             };
            int result = Db.DbHealthService.ExecuteSP("pack_jiekou.UpZixun", parameters, "v_rt");
            return Json(new { status = true, result = new { id = result } }, JsonRequestBehavior.AllowGet);
        }
        //咨询内容图片上传
        public JsonResult add_question_file()
        {
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
                else if (file.ContentLength > 0)
                {
                    int MaxContentLength = 1024 * 1024 * 3; //3 MB
                    string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".pdf" };

                    if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                    }

                    else if (file.ContentLength > MaxContentLength)
                    {
                        ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                    }
                    else
                    {
                        //TO:DO

                        var path = System.Web.HttpContext.Current.Server.MapPath("~" + "file");
                        file.SaveAs(path);
                        ModelState.Clear();
                        ViewBag.Message = "File uploaded successfully";
                    }
                }
            }
            return View();
        }
        #endregion
        #region PE Files
        #endregion
    }
    public class ReturnStatus
    {
        public bool status;
        public object result;
        public String message;
    }
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }
        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
            return DateTime.Now.ToString("yyyyMMddhhmmss") + name.Replace("\"", string.Empty);
        }
    }
}
