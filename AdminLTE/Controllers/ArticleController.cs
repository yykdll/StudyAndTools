using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdminLTE.Domain.Services;
using AdminLTE.Enum;
using AdminLTE.Filter;
using AdminLTE.Helper;
using MvcBase.Controls;
using PagedList;

namespace AdminLTE.Controllers
{

    /// <summary>
    /// 文章
    /// </summary>
    public class ArticleController : Controller
    {
        private readonly IMainDBTool _dbTool;
        private readonly IArticleService _articleService;
        private readonly IArticleClassifyService _articleClassifyService;
        private readonly LoginInfo _loginInfo;
        public ArticleController(IMainDBTool dbTool, IArticleService articleService, IArticleClassifyService articleClassifyService)
        {
            this._dbTool = dbTool;
            this._articleService = articleService;
            this._articleClassifyService = articleClassifyService;
            this._loginInfo = LoginInfoHelper.Current();
        }


        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        [PermissionValidate(PermissionType.Index)]
        public ActionResult Index(int? page, string parenetclassifyID, string classifyID, string keyword)
        {
            var list_Article = _articleService.List().OrderByDescending(m => m.CreateTime)
                .Where(s => s.IsDelete != true)//不显示删除的文章
                .WhereIf(s => s.ParentClassifyID == parenetclassifyID, !string.IsNullOrEmpty(parenetclassifyID))
                .WhereIf(s => s.ClassifyID == classifyID, !string.IsNullOrEmpty(classifyID))
                .WhereIf(s => s.Title.Contains(keyword) || s.Description.Contains(keyword) || s.Keywords.Contains(keyword), !string.IsNullOrEmpty(keyword));

            return View(list_Article.ToList());
        }

        [PermissionValidate(PermissionType.Edit, PermissionType.Create)]
        public ActionResult Edit(string ID, string classifyID)
        {
            var model = _articleService.SingleAndInit(ID);
            if (!string.IsNullOrEmpty(classifyID))
            {
                var o_classifyService = _articleClassifyService.SingleAndInit(m => m.ParentClassifyID == classifyID);
                model.ClassifyID = o_classifyService.ID;
                model.ParentClassifyID = o_classifyService.ParentClassifyID;
            }
            if (model.ID == "0"||string.IsNullOrEmpty(model.ID))
            {
                model.CreatorID = _loginInfo.ID;
                model.CreatorName = _loginInfo.UserName;
            }
            return View(model);
        }

        [PermissionValidate(PermissionType.Edit, PermissionType.Create)]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Save(string ID,bool isPublish=true)
        {
            ReturnResult result = new ReturnResult();
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _articleService.SingleAndInit(ID);

                    #region 更新发布时间

                    //如果发布选项有变化
                    if ((entity.IsPublish == false && isPublish == true)||entity.IsPublish==null)
                    {
                        if (RouteData.CheckRole(PermissionType.Publish) == false)
                        {
                            result.Message = "您没有发布权限！";
                            return Json(result);
                        }
                        entity.PublishTime = DateTime.Now;
                        entity.IsPublish = true;
                        entity.PublisherID = _loginInfo.UserName;
                    }
                    else if (entity.IsPublish == true && isPublish == false)
                    {
                        if (RouteData.CheckRole(PermissionType.Publish) == false)
                        {
                            result.Message = "您没有发布权限！";
                            return Json(result);
                        }
                        entity.PublishTime = null;
                        entity.IsPublish = false;
                    }

                    #endregion

                    TryUpdateModel(entity, null, Request.Form.AllKeys, new string[] {"IsPublish"});
                    if (string.IsNullOrEmpty(ID))
                    {
                        entity.CreatorID = _loginInfo.ID;
                        entity.CreatorName = _loginInfo.UserName;
                    }
                    entity.LastEditTime = DateTime.Now;
                    _articleService.Save(entity, entity.ID);
                    _dbTool.Commit();
                    result.Message = "保存成功";
                    result.Status = 200;
                }
                catch (Exception ex)
                {
                    result.Message = ex.ToString();
                }
            }
            else
            {
                result.Status = 500;
                result.Message = "保存失败";
            }
            return Json(result);
        }



        ////删除文章
        //[PermissionValidate(PermissionType.Delete)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Delete(int articleID)
        //{
        //    ReturnResult result = new ReturnResult();
        //    try
        //    {
        //        var entity = _articleService.Single(articleID);
        //        if (entity != null)
        //        {
        //            entity.IsDeleted = true;
        //            _articleService.Save(entity, entity.ArticleID);
        //            _dbTool.Commit();
        //            result.Status = 200;
        //        }
        //        else
        //        {
        //            result.Message = "找不到对应文章";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.ToString();
        //    }
        //    return Json(result);
        //}

        ////撤销删除文章
        //[PermissionValidate(PermissionType.Delete)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult UndoDel(int articleID)
        //{
        //    ReturnResult result = new ReturnResult();
        //    try
        //    {
        //        var entity = _articleService.Single(articleID);
        //        if (entity != null)
        //        {
        //            entity.IsDeleted = false;
        //            _articleService.Save(entity, entity.ArticleID);
        //            _dbTool.Commit();
        //            result.Status = 200;
        //        }
        //        else
        //        {
        //            result.Message = "找不到对应文章";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.ToString();
        //    }
        //    return Json(result);
        //}


        ////发布文章
        //[PermissionValidate(PermissionType.Publish)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Publish(int articleID)
        //{
        //    ReturnResult result = new ReturnResult();
        //    try
        //    {
        //        var entity = _articleService.Single(articleID);
        //        if (entity != null)
        //        {
        //            entity.IsPublished = true;
        //            entity.PublishTime = DateTime.Now;
        //            entity.Publisher = _loginInfo.UserName;
        //            _articleService.Save(entity, entity.ArticleID);
        //            _dbTool.Commit();
        //            result.Status = 200;
        //        }
        //        else
        //        {
        //            result.Message = "找不到对应文章";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.ToString();
        //    }
        //    return Json(result);
        //}

        ////撤销删除文章
        //[PermissionValidate(PermissionType.Publish)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult UndoPublish(int articleID)
        //{
        //    ReturnResult result = new ReturnResult();
        //    try
        //    {
        //        var entity = _articleService.Single(articleID);
        //        if (entity != null)
        //        {
        //            entity.IsPublished = false;
        //            entity.PublishTime = null;
        //            entity.Publisher = string.Empty;
        //            _articleService.Save(entity, entity.ArticleID);
        //            _dbTool.Commit();
        //            result.Status = 200;
        //        }
        //        else
        //        {
        //            result.Message = "找不到对应文章";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.ToString();
        //    }
        //    return Json(result);
        //}

        ////置顶文章
        //[PermissionValidate(PermissionType.Publish)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult PlacedTop(int articleID, int topicID)
        //{
        //    ReturnResult result = new ReturnResult();
        //    try
        //    {
        //        var entity = _articleService.Single(articleID);
        //        if (entity != null)
        //        {
        //            if (!entity.IsPublished.GetValueOrDefault(false))
        //            {
        //                result.Message = "先发布才能置顶！";
        //                return Json(result);

        //            }
        //            else
        //            {
        //                if (string.IsNullOrWhiteSpace(entity.BigPicAddress))
        //                {
        //                    result.Message = "文章置顶封面为空不能置顶，请先上传图片！";
        //                    return Json(result);
        //                }
        //            }
        //            entity.PlacedTopFlag = true;
        //            _articleService.Save(entity, entity.ArticleID);

        //            var entity_placedTop = _articleService.List(m => m.PlacedTopFlag == true && m.TopicID == topicID).FirstOrDefault();
        //            if (entity_placedTop != null)
        //            {
        //                entity_placedTop.PlacedTopFlag = false;
        //                _articleService.Update(entity_placedTop);
        //            }

        //            _dbTool.Commit();
        //            result.Status = 200;
        //        }
        //        else
        //        {
        //            result.Message = "找不到对应文章";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.ToString();
        //    }
        //    return Json(result);
        //}
        ////置顶文章
        //[PermissionValidate(PermissionType.Publish)]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult UndoPlacedTop(int articleID)
        //{
        //    ReturnResult result = new ReturnResult();
        //    try
        //    {
        //        var entity = _articleService.Single(articleID);
        //        if (entity != null)
        //        {
        //            entity.IsTop = false;
        //            _articleService.Save(entity, entity.ArticleID);
        //            _dbTool.Commit();
        //            result.Status = 200;
        //        }
        //        else
        //        {
        //            result.Message = "找不到对应文章";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.ToString();
        //    }
        //    return Json(result);
        //}
    }
}