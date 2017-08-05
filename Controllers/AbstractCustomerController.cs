using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YsrisCoreLibrary.Dal;
using YsrisCoreLibrary.Enums;
using YsrisCoreLibrary.Helpers;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;
using YsrisSaas2.Models;
using YsrisCoreLibrary.Abstract;

namespace YsrisCoreLibrary.Controllers
{
    /// <summary>
    /// Default customer management
    /// </summary>
    public abstract class AbstractCustomerController : AbstractController
    {
        protected readonly MailHelperService _mailHelperService;
        protected readonly IHostingEnvironment _env;
        protected readonly ILogger<AbstractCustomerController> _myLogger;
        protected readonly SessionHelperService _sessionHelperInstance;

        protected AbstractCustomerDal _dal;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sessionHelper"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        /// <param name="mailHelperService"></param>
        public AbstractCustomerController(SessionHelperService sessionHelper,
            ILogger<AbstractCustomerController> logger, IHostingEnvironment env, MailHelperService mailHelperService)
        {
            _sessionHelperInstance = sessionHelper;
            _myLogger = logger;
            _env = env;
            _mailHelperService = mailHelperService;   
            _dal = new CustomerDal();         
        }

        /// <summary>
        /// account creation action
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public virtual Customer Post([FromBody] dynamic values)
        {
            _myLogger.LogDebug($"CustomerController +Post");

            var email = values.email;

            var test = _dal.SafeList(0).Where(a => a.email == email.ToString());
            if (test.Any())
                throw new Exception("Already assigned email");

            var rolesList = new List<Role>();
            Role customerType = Role.User;
            switch ((string)values.UserType.ToString())
            {
                case "Coach":
                    rolesList.Add(Role.Coach);
                    customerType = Role.Coach;
                    break;
                case "User":
                    rolesList.Add(Role.User);
                    customerType = Role.User;
                    break;
                case "Proprietaire":
                    rolesList.Add(Role.Proprietaire);
                    customerType = Role.Proprietaire;
                    break;
                case "Locataire":
                    rolesList.Add(Role.Locataire);
                    customerType = Role.Locataire;
                    break;
                case "Business":
                    rolesList.Add(Role.Business);
                    customerType = Role.Business;
                    break;                    
            }

            var entity = new Customer(values)
            {
                activationCode = Guid.NewGuid().ToString(),
                customerType = customerType,
                createdAt = DateTime.Now,
                accountStatus = CustomerStatus.PendingActivationWithoutPasswordChange,
                rolesString = string.Join(",", rolesList.Select(a => a.ToString())),
                id = 0
            };
            entity.password = new EncryptionHelper().GetHash(values.passwordForTyping.ToString()); //keep here for avoiding the model binding
            entity.customerMainAdressId = (int)new PostalAddressDal().AddOrUpdate(entity.customerMainAdress ?? new PostalAddress(), 0);
            entity.companyId = 0;
            entity.id = (int)_dal.AddOrUpdate(entity, 0);

            // 4. User notification
            _myLogger.LogDebug($"+++User notification (mail)");
            _mailHelperService.SendMail(
                entity.email,
                subject: "Confirm account creation",
                templateUri: _env.ContentRootPath + "/Views/Emails/SignUpConfirmation.cshtml",
                mailViewBag:
                new Dictionary<string, string>
                {
                    {"FirstName", entity.firstName},
                    {
                        "ActivationUrl",
                        $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/activation/Activate?username={entity.email}&activationCode={entity.activationCode}"
                    }
                }
            );

            return entity;
        }

        /// <summary>
        /// Upload avatar of connected user
        /// </summary>
        /// <returns></returns>
        [HttpPost("avatar")]
        [Authorize]
        public virtual object UploadAvatar(IFormFile file)
        {
            if (!HttpContext.Request.Form.Files.Any())
                return null;

            //var fullPath = FileSystemHelper.SaveTo(buffer, $"/uploads/avatars/full");
            //var smallPath = FileSystemHelper.SaveTo(buffer, $"/uploads/avatars/small", 150, 150);
            var largePath = $"/uploads/avatars/large/{_sessionHelperInstance.User.id}.jpg";
            FileSystemHelper.SavePictureTo(file, largePath, 1000, 800);

            var entity = _dal.Get(_sessionHelperInstance.User.id, _sessionHelperInstance.User.id);
            entity.picture = largePath;

            _dal.AddOrUpdate(entity, _sessionHelperInstance.User.id);

            return entity;
        }

        /// <summary>
        /// account update action
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("update")]
        [Authorize]
        public virtual Customer Update([FromBody] dynamic values)
        {
            var entity = _dal.Get(_sessionHelperInstance.User.id, _sessionHelperInstance.User.id);
            entity.SetFromValues(values);

            //var entity = new Customer(values);
            if (entity.id != _sessionHelperInstance.User.id)
                throw new Exception("Unauthorized");


            entity.customerMainAdressId = (int)new PostalAddressDal().AddOrUpdate(entity.customerMainAdress ?? new PostalAddress(), 0);

            if (values.rawPasswordConfirm != null && values.passwordForTyping != null)
                if (!string.IsNullOrEmpty(values.rawPasswordConfirm.ToString()) &&
                    !string.IsNullOrEmpty(values.passwordForTyping.ToString()))
                {
                    if (values.rawPasswordConfirm.ToString() != values.passwordForTyping.ToString())
                        throw new Exception("Password confirmation mismatch");
                    entity.password = new EncryptionHelper().GetHash(values.passwordForTyping.ToString());
                }

            entity.id = (int)_dal.AddOrUpdate(entity, _sessionHelperInstance.User.id);
            _sessionHelperInstance.HttpContext.Session.SetString("UserEntity",
                (string)JsonConvert.SerializeObject(entity));

            return entity;
        }

        /// <summary>
        /// Get connected user account action
        /// </summary>
        /// <returns>Customer</returns>
        [HttpGet("me")]
        [Authorize]
        //[ServiceFilter(typeof(CustomAuthorize))]
        public virtual Customer GetMe()
        {
            if (_sessionHelperInstance.User == null)
                return null;
            var entity = _dal.Get(_sessionHelperInstance.User.id, _sessionHelperInstance.User.id);
            if (entity != null)
            {
                entity.pictureClientAccessor = $"/api/customer/avatar/{entity.id}";
            }
            return entity;
        }

        /// <summary>
        /// Get user account action
        /// </summary>
        /// <remarks>
        /// Maybe user filtration should be needed to override for this action...
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Customer</returns>
        [HttpGet("{id}")]
        [Authorize]
        public virtual object Get(int id)
        {
            var entity = _dal.Get(id, _sessionHelperInstance.User.id);
            if (entity != null)
            {
                entity.pictureClientAccessor = $"/api/customer/avatar/{entity.id}";
            }
            return entity;
        }

        /// <summary>
        /// Get connected user avatar file content action
        /// </summary>
        /// <returns></returns>
        [HttpGet("avatar")]
        [Authorize]
        public virtual IActionResult GetAvatar()
        {
            if (_sessionHelperInstance.User == null)
                return null;
            int id = _sessionHelperInstance.User.id;

            var smallUri = _dal.Get((int)id, _sessionHelperInstance.User.id).picture;


            if (smallUri == null)
            {
                var path = Path.Combine(_env.WebRootPath, "bobos_components/assets/images/profile-placeholder.png");
                return File(System.IO.File.ReadAllBytes(path), "image/png");
            }

            return File(FileSystemHelper.GetFileContent(smallUri).Result.ToArray(), "image/jpeg");
        }

        /// <summary>
        /// Get any user avatar file content action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("avatar/{id}")]
        [Authorize]
        public virtual IActionResult GetAvatar(int id)
        {
            var smallUri = _dal.Get((int)id, _sessionHelperInstance.User.id).picture;
            if (smallUri == null)
            {
                var path = Path.Combine(_env.WebRootPath, "bobos_components\\assets\\images\\profile-placeholder.png");
                return File(System.IO.File.ReadAllBytes(path), "image/png");
            }
            return File(FileSystemHelper.GetFileContent(smallUri).Result.ToArray(), "image/jpeg");

        }

        /// <summary>
        /// Delete user account
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public void Delete()
        {
            var entity = _dal.Get(_sessionHelperInstance.User.id, _sessionHelperInstance.User.id);
            _dal.SafeRemove(entity, _sessionHelperInstance.User.id);
        }

        [HttpGet("hello")]
        public string HelloWorld()
        {
            return $"HelloWorld {_sessionHelperInstance.ConnectedClientProjectCode}";
        }
    }
}
