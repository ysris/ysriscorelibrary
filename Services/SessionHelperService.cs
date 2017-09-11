using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Services
{
    public class SessionHelperService
    {
        private readonly IHttpContextAccessor _accessor;

        public SessionHelperService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public HttpContext HttpContext => _accessor.HttpContext;

        public Customer User
        {
            get
            {
                var str = _accessor.HttpContext.Session.GetString("UserEntity");
                if (str != null)
                    return JsonConvert.DeserializeObject<Customer>(str);
                return null;
            }
        }

        public string ConnectedClientProjectCode
        {
            get { return _accessor.HttpContext.Session.GetString("ConnectedClientProjectCode"); }
            set { _accessor.HttpContext.Session.SetString("ConnectedClientProjectCode", value); }
        }
    }
}
