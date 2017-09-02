using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YsrisCoreLibrary.Abstract;
using YsrisCoreLibrary.Dal;
using YsrisCoreLibrary.Enums;

namespace YsrisCoreLibrary.Controllers
{
    public class ActivationController : AbstractController
    {
        public static AbstractCustomerDal dal;
        private IHostingEnvironment Env;
        private readonly ILogger<ActivationController> MyLogger;

        public ActivationController(ILogger<ActivationController> logger, IHostingEnvironment env)
        {
            MyLogger = logger;
            Env = env;
            dal = new CustomerDal();
        }

        

        public IActionResult Activate()
        {
            // Moved into CustomerController
            throw new NotImplementedException();
            // var activatioNCode = Request.Query["activationCode"];
            // var username = Request.Query["username"];

            // var entity = dal.Get(username, 0);

            // switch (entity.accountStatus)
            // {
            //     case CustomerStatus.PendingActivationWithoutPasswordChange:
            //         if (entity.activationCode == activatioNCode)
            //         {
            //             entity.activationCode = null;
            //             entity.accountStatus = CustomerStatus.Activated;
            //             dal.AddOrUpdate(entity, 0);
            //             MyLogger.LogDebug(
            //                 $"++ Updatied entity AccountStatus from {CustomerStatus.PendingActivationWithoutPasswordChange} to {entity.accountStatus}");
            //             return Redirect("/#!/signin/activationsucceeded");
            //         }
            //         return Content("Activation error");
            //     default:
            //         throw new NotImplementedException();
            // }
        }
    }
}