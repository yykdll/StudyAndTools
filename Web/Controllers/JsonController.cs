using MvcBase.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.Helper;

namespace Web.Controllers
{
    public class JsonController : Controller
    {
        /// <summary>
        /// Json格式化
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View();
        }
        /// <summary>
        /// 格式化json
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult JsonFormat(string content)
        {
            ReturnResult<string> result = new ReturnResult<string>();
            result = JsonHelper.JsonFormat(content);
            return Json(result);
        }
    }
}
