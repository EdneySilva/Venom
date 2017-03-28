using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Venom.Lib.Data;
using Venom.Web.Helper;

namespace Venom.Web
{
    public class JsonSerializerContext : IDisposable
    {
        private int MaxDepth { get; set; }

        public JsonSerializerContext(int maxDepth = 2)
        {
            MaxDepth = maxDepth;    
        }

        public JsonResult Json(object data)
        {
            var json = new JsonNetResult();
            json.Settings.MaxDepth = MaxDepth;
            json.Settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            json.Data = data;
            return json;
        }

        public JsonResult Json(object data, JsonRequestBehavior behavior)
        {
            var json = new JsonNetResult();
            json.Settings.MaxDepth = 2;
            json.Data = data;
            json.Settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            json.JsonRequestBehavior = behavior;
            return json;
        }

        public void Dispose()
        {
        }
    }
}
