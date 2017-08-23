using MvcBase.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helper
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// json格式化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public ReturnResult<string> JsonFormat(string str)
        {
            ReturnResult<string> result = new ReturnResult<string>();
            try
            {
                //格式化json字符串  
                JsonSerializer serializer = new JsonSerializer();
                TextReader tr = new StringReader(str);
                JsonTextReader jtr = new JsonTextReader(tr);
                object obj = serializer.Deserialize(jtr);
                if (obj != null)
                {
                    StringWriter textWriter = new StringWriter();
                    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 4,
                        IndentChar = ' '
                    };
                    serializer.Serialize(jsonWriter, obj);
                    result.Status = 200;
                    result.Message = "成功";
                    result.Data = textWriter.ToString();
                    return result;
                }
                else
                {
                    result.Status = 500;
                    result.Message = "转换失败！";
                    result.Data = str;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Status = 500;
                result.Message = e.Message;
                return result;
            }
        }
    }
}
