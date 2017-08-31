using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdminLTE.Domain.Services;

namespace Web.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            this._articleService = articleService;
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var list_Article = _articleService.List(m => m.IsPublish.GetValueOrDefault(false) && !m.IsDelete.GetValueOrDefault(false)).ToList();
            return View(list_Article);
        }
    }
}