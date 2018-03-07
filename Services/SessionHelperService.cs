using Microsoft.AspNetCore.Http;
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
    public class SessionHelperService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly CustomerDal _dal;

        public SessionHelperService(IHttpContextAccessor accessor, CustomerDal dal)
        {
            _accessor = accessor;
            _dal = dal;
        }

        public HttpContext HttpContext => _accessor.HttpContext;

        public ICustomer User
        {
            get
            {
                //try
                //{
                    var str = _accessor.HttpContext.Session.GetString("UserEntity");
                    if (str != null)
                        return JsonConvert.DeserializeObject<Customer>(str);
                    else
                    {
                        var email = HttpContext.User.Claims.First().Value;
                        return _dal.Get(email, 0);
                    }
                //}
                //catch
                //{
                //    return null;
                //}
            }
        }

        public string ConnectedClientProjectCode
        {
            get { return _accessor.HttpContext.Session.GetString("ConnectedClientProjectCode"); }
            set { _accessor.HttpContext.Session.SetString("ConnectedClientProjectCode", value); }
        }
    }
}
