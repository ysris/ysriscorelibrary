using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using YsrisCoreLibrary.Models;
using YsrisSaas2.Models;

namespace YsrisCoreLibrary.Abstract
{
    public class AbstractController : Controller
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    //Manage user access here

        //    var area = context.Controller.GetType().GetTypeInfo().GetCustomAttributes(typeof(AreaAttribute)).Select(b => ((AreaAttribute)b).RouteValue).SingleOrDefault() ?? "";
        //    var controller = context.Controller.GetType().Name.ToString();
        //    var action = ((dynamic)context.ActionDescriptor).ActionName;
        //    var httpMethod = "Http" + context.HttpContext.Request.Method.ToString();            

        //    var actionDecorators = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttribute(typeof(AuthorizeAttribute));

        //    if (actionDecorators != null)
        //    {
        //        var userEntity = JsonConvert.DeserializeObject<Customer>(context.HttpContext.Session.GetString("UserEntity"));

        //        var correspondingSet =
        //            from x in userEntity.userRights
        //            where x.areaName == area
        //            where x.controllerName == controller
        //            where x.actionName == action
        //            where x.httpMethod.ToLower() == httpMethod.ToLower()
        //            select x;

        //        //TODO : Review
        //        if (correspondingSet.Count() != 1 && !userEntity.roles.Contains(Role.Administrator))
        //        {
        //            throw new Exception("User doesn't have the access  right to this resource");
        //        }
        //    }

        //    base.OnActionExecuting(context);
        //}
    }
}
