using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdminLTE.Domain.Services;
using AdminLTE.Enum;
using AdminLTE.Filter;
using MvcBase.Controls;

namespace AdminLTE.Controllers
{
    public class ArticleClassifyController : BaseController
    {
        private readonly IMainDBTool _dbTool;
        private readonly IArticleClassifyService _articleClassifyService;
        private readonly IPermissionService _permissionService;

        public ArticleClassifyController(IArticleClassifyService articleClassifyService, IMainDBTool dbTool, IPermissionService permissionService)
        {
            this._articleClassifyService = articleClassifyService;
            this._dbTool = dbTool;
            this._permissionService = permissionService;
        }
        [PermissionValidate(PermissionType.Index)]
        public ActionResult Index()
        {
            var list = _articleClassifyService
                .List(m=>m.IsDelete!=true)
                .OrderBy(s => s.OrderID)
                .ToList();
            return View(list);
        }
        /// <summary>
        /// 添加或者编辑文章分类
        /// </summary>
        /// <returns></returns>
        [PermissionValidate(PermissionType.Create,PermissionType.Edit)]
        public ActionResult Edit(string ID, string parentID)
        {
            var o_articleClassify = _articleClassifyService.SingleAndInit(ID);
            if (!string.IsNullOrEmpty(parentID) && parentID != "0" && parentID != "undifined") o_articleClassify.ParentClassifyID = parentID;
            //ViewBag.Permissions = typeof(PermissionType).GetEnumListItem();
            return View(o_articleClassify);
        }
        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [PermissionValidate(PermissionType.Edit, PermissionType.Create)]
        public ActionResult Save(string id)
        {
            ReturnResult result = new ReturnResult();
            if (ModelState.IsValid)
            {
                try
                {
                    var o_articleClassify = _articleClassifyService.SingleAndInit(id);
                    TryUpdateModel(o_articleClassify, null, Request.Form.AllKeys);
                    _articleClassifyService.Save(o_articleClassify, id);
                    _dbTool.Commit();
                    result.Status = 200;
                    result.Message = "保存成功!";
                }
                catch (Exception ex)
                {
                    result.Message = ex.ToString();
                }
            }
            else
            {
                result.Status = 500;
                result.Message = "保存失败!";
            }
            return Json(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [PermissionValidate(PermissionType.Delete)]
        public JsonResult Delete(string ID)
        {
            ReturnResult result = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(ID) && ID != "0" && ID != "undifined")
                {
                    var list_articleClassify=_articleClassifyService.List(s => s.ParentClassifyID == ID);
                    foreach (var item in list_articleClassify)
                    {
                        item.IsDelete = true;
                        _articleClassifyService.Save(item, ID);
                    }
                    var o_articleClassify = _articleClassifyService.SingleAndInit(ID);
                    o_articleClassify.IsDelete = true;
                    _articleClassifyService.Save(o_articleClassify, ID);
                    _dbTool.Commit();
                    result.Status = 200;
                    result.Message = "删除成功";
                }
                else
                {
                    result.Status = 500;
                    result.Message = "菜单不存在，删除失败!";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
            }
            return Json(result);
        }
    }
}