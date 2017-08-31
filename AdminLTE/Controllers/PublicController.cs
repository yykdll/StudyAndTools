using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdminLTE.Domain.Services;
using MvcBase.Controls;

namespace AdminLTE.Controllers
{
    public class PublicController : Controller
    {
        private readonly IArticleClassifyService _classifyService;
        public PublicController(IArticleClassifyService classifyService)
        {
            this._classifyService = classifyService;
        }
        public FileResult ValidateCode(int? w, int? h)
        {
            var code = ValidateWhiteBlackImgCode.RandemCode();
            Session["ValidateCode"] = code;
            return File(ValidateWhiteBlackImgCode.Img(code, w ?? 200, h ?? 75), "image/png");
        }

        public ActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult TopicList(string PID)
        {
            ReturnResult<List<SelectListItem>> result = new ReturnResult<List<SelectListItem>>();
            try
            {
                result.Data = _classifyService.DropDownList(PID);
                result.Status = 200;
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
            }
            return Json(result);
        }
    }
}