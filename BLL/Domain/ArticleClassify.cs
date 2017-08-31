using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AdminLTE;
using MvcBase.Infrastructure;

namespace AdminLTE.Domain
{
    public partial class MainDbContext
    {
        public DbSet<ArticleClassify> ArticleClassify { get; set; }
    }
    /// <summary>
    /// 文章分类
    /// </summary>
    [Table("ARTICLECLASSIFY")]
    public class ArticleClassify
    {
        public ArticleClassify()
        {
            this.CreateTime=DateTime.Now;
            this.ID = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 分类ID
        /// </summary>
        [Key]
        [DisplayName("分类ID")]
        [Column("ID")]
        public String ID { get; set; }
        /// <summary>
        /// 分类名
        /// </summary>
        [DisplayName("分类名")]
        [Column("CLASSIFYNAME")]
        public String ClassifyName { get; set; }
        /// <summary>
        /// 父栏目ID
        /// </summary>
        [DisplayName("父分类ID")]
        [Column("PARENTCLASSIFYID")]
        public String ParentClassifyID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        [Column("CREATETIME")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        [Column("ORDERID")]
        public Int32? OrderID { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [DisplayName("是否删除")]
        [Column("ISDELETE")]
        public Boolean? IsDelete { get; set; }
    }
    namespace Services
    {
        public interface IArticleClassifyService : IServiceBase<ArticleClassify>
        {
            List<SelectListItem> DropDownList(string PID);
            List<ArticleClassify> ListCache();
            void ClearCache();
        }
        public class ArticleClassifyService : ServiceBase<ArticleClassify>, IArticleClassifyService
        {
            public ArticleClassifyService(IMainDbFactory factory) : base(factory) { }
            public List<SelectListItem> DropDownList(string PID)
            {
                return ListCache()
                    .Where(s => s.IsDelete != true)
                    .WhereIf(s => s.ParentClassifyID == PID, !string.IsNullOrEmpty(PID))
                    .WhereIf(s => s.ParentClassifyID == "0" || s.ParentClassifyID == null, string.IsNullOrEmpty(PID))
                    .Select(s => new SelectListItem() { Text = s.ClassifyName, Value = s.ID + "" })
                    .ToList()
                    ;
            }
            string cacheName = "ArticleClassify";
            public List<ArticleClassify> ListCache()
            {
                return ListCache(cacheName, MvcBase.Enum.CacheTimeType.ByMinutes, 3, s => s.IsDelete != true);
            }

            public void ClearCache()
            {
                MvcBase.Helper.CacheExtensions.ClearCache(cacheName);
            }
        }
    }
}
