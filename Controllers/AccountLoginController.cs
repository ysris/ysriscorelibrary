using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using YsrisCoreLibrary.Dal;
using YsrisCoreLibrary.Enums;
using YsrisCoreLibrary.Helpers;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;
using System.Linq;

namespace YsrisCoreLibrary.Controllers
{

    public abstract class AbstractAccountLoginController : Controller
    {

        protected AbstractCustomerDal dal; //TODO = new CustomerDal();
        private SessionHelperService SessionHelperInstance;
        private ILogger<AbstractAccountLoginController> MyLogger;
        private IHostingEnvironment Env;
        private readonly MailHelperService mailHelperService;

        public AbstractAccountLoginController(SessionHelperService sessionHelper, ILogger<AbstractAccountLoginController> logger, IHostingEnvironment env, MailHelperService _mailHelperService)
        {
            SessionHelperInstance = sessionHelper;
            MyLogger = logger;
            Env = env;
            mailHelperService = _mailHelperService;
        }

        [HttpGet("Login")]
        public string Login()
        {
            return "FUU";
        }

        [HttpPost("Login")]
        public async Task<Customer> Login([FromBody] dynamic values)
        {
            var entity = dal.Get((string)values.username.ToString(), (string)values.password.ToString());

            if (entity == null)
                throw new Exception("UnknownUser");

            var fullEntity = dal.Get((int)entity.Item1, (int)entity.Item1);
            //fullEntity.MenuItems = new CustomerModuleDal().ListModules(fullEntity, entity.Item1 ).Select(a => new MenuItem { }).ToList();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, entity.Item2) };
            if (!string.IsNullOrEmpty(fullEntity.rolesString))
                foreach (var cur in fullEntity.rolesString.Split(',').Select(a => a.Trim()))
                    claims.Add(new Claim(ClaimTypes.Role, cur));
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

            await HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance", principal);


            SessionHelperInstance.HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(fullEntity));


            return fullEntity;
        }

        [HttpPost("Logout")]
        public async void Logout()
        {
            await HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");
        }

        [HttpPost("recover")]
        [AllowAnonymous]
        public void Recover([FromBody]dynamic obj)
        {
            string email = obj?.email;
            if (email == null)
                throw new Exception("incorrect parameters specified");

            var entity = dal.Get(email, 0);
            entity.activationCode = Guid.NewGuid().ToString();
            entity.recoverAskDate = DateTime.Now;
            entity.accountStatus = CustomerStatus.PendingActivationWithPasswordChange;

            dal.AddOrUpdate(entity, 0);

            mailHelperService.SendMail(
                entity.email,
                subject: "Password recover",
                templateUri: Env.ContentRootPath + "\\Views\\Emails\\UserPasswordReset.cshtml",
                mailViewBag:
                new Dictionary<string, string>
                {
                    {"UserFirstName", entity.firstName},
                    {
                        "RecoverUrl",
                        $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/#!/passwordrecover2/{entity.email}/{entity.activationCode}"
                    }
                }
            );
        }

        [HttpPost]
        [Route("recover2")]
        [AllowAnonymous]
        public void Recover2([FromBody]dynamic obj)
        {
            string
                email = obj.email,
                activationCode = obj.activationCode,
                password = obj.password;

            var entity = dal.Get(email, 0);
            if (entity == null || entity.activationCode == null || entity.recoverAskDate == null)
                throw new Exception("BadRequest");

            if (activationCode == entity.activationCode && (DateTime.Now - (DateTime)entity.recoverAskDate).Minutes <= 10)
            {
                entity.activationCode = null;
                entity.recoverAskDate = null;
                entity.password = new EncryptionHelper().GetHash(password);
                entity.accountStatus = CustomerStatus.Activated;
                dal.AddOrUpdate(entity, 0);

                mailHelperService.SendMail(
                    entity.email,
                    subject: "Password recover",
                    templateUri: Env.ContentRootPath + "\\Views\\Emails\\UserPasswordResetConfirmation.cshtml",
                    mailViewBag:
                    new Dictionary<string, string>
                    {
                        {"UserFirstName", entity.firstName},
                        //{
                        //    "RecoverUrl",
                        //    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/#!/signin"
                        //}
                    }
                );
            }
        }
    }
}