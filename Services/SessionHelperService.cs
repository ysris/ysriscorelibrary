using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Dal;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Services
{
    public class SessionHelperService<T> where T : class, ICustomer
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly DbContext _context;

        public SessionHelperService(IHttpContextAccessor accessor, DbContext context)
        {
            _accessor = accessor;
            _context = context;
        }

        public HttpContext HttpContext => _accessor.HttpContext;

        public T User
        {
            get
            {
                var str = _accessor.HttpContext.Session.GetString("UserEntity");
                if (str != null)
                    return JsonConvert.DeserializeObject<T>(str);
                else
                {
                    var email = HttpContext.User.Claims.First().Value;
                    return _context.Set<T>().Single(a => a.email == email);
                }
            }
        }

        public string ConnectedClientProjectCode
        {
            get { return _accessor.HttpContext.Session.GetString("ConnectedClientProjectCode"); }
            set { _accessor.HttpContext.Session.SetString("ConnectedClientProjectCode", value); }
        }
    }
}
