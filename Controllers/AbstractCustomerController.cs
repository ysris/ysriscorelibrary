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
using YsrisCoreLibrary.Abstract;
using ysriscorelibrary.Interfaces;
using YsrisSaas2.Enums;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace YsrisCoreLibrary.Controllers
{
    /// <summary>
    /// Default customer management
    /// </summary>
    public abstract class AbstractCustomerController : Controller
    {
        protected readonly MailHelperService _mailHelperService;
        protected readonly IHostingEnvironment _env;
        protected readonly ILogger<AbstractCustomerController> _myLogger;
        protected readonly SessionHelperService _sessionHelperInstance;
        protected readonly IStorageService _storageService;
        protected AbstractCustomerDal _dal;
        private IConfiguration _config;
        private EncryptionService _encryptionHelper;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sessionHelper"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        /// <param name="mailHelperService"></param>
        public AbstractCustomerController(
            SessionHelperService sessionHelper,
            ILogger<AbstractCustomerController> logger,
            IHostingEnvironment env,
            MailHelperService mailHelperService,
            IStorageService storageService,
            IConfiguration config,
            CustomerDal dal,
            EncryptionService encryptionHelper
        )
        {
            _sessionHelperInstance = sessionHelper;
            _myLogger = logger;
            _env = env;
            _mailHelperService = mailHelperService;
            _storageService = storageService;
            _dal = dal;
            _config = config;
            _encryptionHelper = encryptionHelper;
        }

        public class LoginCustomerEntity
        {
            public Customer customer { get; set; }            
        }


        [HttpPost("Login")]
        public virtual async Task<LoginCustomerEntity> Login([FromBody] dynamic values)
        {
            var customer = _dal.Get((string)values.username.ToString(), (string)values.password.ToString());

            if (customer == null)
                throw new Exception("Unknown User");

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, customer.email) };
            if (!string.IsNullOrEmpty(customer.rolesString))
                foreach (var cur in customer.rolesString.Split(',').Select(a => a.Trim()))
                    claims.Add(new Claim(ClaimTypes.Role, cur));
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

            // await HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance", principal);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //_sessionHelperInstance.HttpContext
            HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(customer));
            return new LoginCustomerEntity { customer = customer };
        }

        [HttpPost("Logout")]
        public virtual async void Logout() => await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        [HttpPost("recover")]
        [AllowAnonymous]
        public virtual void Recover([FromBody]dynamic obj)
        {
            string email = obj?.email;
            if (email == null)
                throw new Exception("incorrect parameters specified");

            var entity = _dal.Get(email, 0);
            entity.activationCode = Guid.NewGuid().ToString();
            entity.recoverAskDate = DateTime.Now;
            entity.accountStatus = CustomerStatus.PendingActivationWithPasswordChange;

            _dal.AddOrUpdate(entity, 0);

            _mailHelperService.SendMail(
                entity.email,
                subject: "Password recover",
                templateUri: _env.ContentRootPath + "\\Views\\Emails\\UserPasswordReset.cshtml",
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
        public virtual void Recover2([FromBody]dynamic obj)
        {
            string
                email = obj.email,
                activationCode = obj.activationCode,
                password = obj.password;

            var entity = _dal.Get(email, 0);
            if (entity == null || entity.activationCode == null || entity.recoverAskDate == null)
                throw new Exception("BadRequest");

            if (activationCode == entity.activationCode && (DateTime.Now - (DateTime)entity.recoverAskDate).Minutes <= 10)
            {
                entity.activationCode = null;
                entity.recoverAskDate = null;
                entity.password = _encryptionHelper.GetHash(password);
                entity.accountStatus = CustomerStatus.Activated;
                _dal.AddOrUpdate(entity, 0);

                _mailHelperService.SendMail(
                    entity.email,
                    subject: "Password recover",
                    templateUri: _env.ContentRootPath + "\\Views\\Emails\\UserPasswordResetConfirmation.cshtml",
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

            var rolesList = new List<string>();
            string customerType = Role.User;


            if (values.UserType != null)
            {
                switch ((string)values.UserType.ToString())
                {
                    case "Coach":
                        rolesList.Add(Role.Coach);
                        customerType = Role.Coach;
                        break;
                    case "User":
                    default:
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
            }
            else
            {
                rolesList.Add(Role.User);
                customerType = Role.User;
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
            if (values.passwordForTyping == null)
                throw new Exception("Password is null");
            entity.password = _encryptionHelper.GetHash(values.passwordForTyping.ToString()); //keep here for avoiding the model binding
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
                        $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/customer/activate?username={entity.email}&activationCode={entity.activationCode}"
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
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual object UploadAvatar(IFormFile file)
        {
            var x = _sessionHelperInstance.User.id;


            _myLogger.LogInformation($"+ UploadAvatar file={file}");

            var largePath = $"/avatars/large/{_sessionHelperInstance.User.id}.jpg";
            _storageService.SavePictureTo(file, largePath, 300);

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
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual Customer Update([FromBody] dynamic values)
        {
            var entity = _dal.Get(_sessionHelperInstance.User.id, _sessionHelperInstance.User.id);
            entity.SetFromValues(values);

            //var entity = new Customer(values);
            if (entity.id != _sessionHelperInstance.User.id)
                throw new Exception("Unauthorized");



            if (values.rawPasswordConfirm != null && values.passwordForTyping != null)
                if (!string.IsNullOrEmpty(values.rawPasswordConfirm.ToString()) &&
                    !string.IsNullOrEmpty(values.passwordForTyping.ToString()))
                {
                    if (values.rawPasswordConfirm.ToString() != values.passwordForTyping.ToString())
                        throw new Exception("Password confirmation mismatch");
                    entity.password = _encryptionHelper.GetHash(values.passwordForTyping.ToString());
                }

            _dal.AddOrUpdate(entity, _sessionHelperInstance.User.id);
            _sessionHelperInstance.HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(entity));

            return entity;
        }

        /// <summary>
        /// Get connected user account action
        /// </summary>
        /// <returns>Customer</returns>
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
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
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
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
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual IActionResult GetAvatar()
        {
            if (_sessionHelperInstance.User != null)
            {
                var smallUri = _dal.Get(_sessionHelperInstance.User.id, _sessionHelperInstance.User.id).picture;
                if (smallUri == null)
                {
                    var path = Path.Combine(_env.WebRootPath, "bobos_components/assets/images/profile-placeholder.png");
                    return File(System.IO.File.ReadAllBytes(path), "image/png");
                }
                var result = _storageService.GetFileContent(smallUri)?.Result?.ToArray();
                if (result == null)
                    return null;
                return File(result, "image/jpeg");
            }
            return null;
        }

        /// <summary>
        /// Get any user avatar file content action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("avatar/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual IActionResult GetAvatar(int id)
        {
            var smallUri = _dal.Get((int)id, _sessionHelperInstance.User.id).picture;
            if (smallUri == null)
            {
                var path = Path.Combine(_env.WebRootPath, "bobos_components\\assets\\images\\profile-placeholder.png");
                return File(System.IO.File.ReadAllBytes(path), "image/png");
            }
            var result = _storageService.GetFileContent(smallUri)?.Result?.ToArray();
            if (result == null)
            return File(result, "image/jpeg");

        }

        /// <summary>
        /// Delete user account
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public void Delete()
        {
            var entity = _dal.Get(_sessionHelperInstance.User.id, _sessionHelperInstance.User.id);
            _dal.SafeRemove(entity, _sessionHelperInstance.User.id);
        }

        /// Now, ovverride of activate method is mandatory to simplify modification
        public virtual IActionResult Activate(string sucessActivationStatus = CustomerStatus.Activated)
        {
            var activatioNCode = Request.Query["activationCode"];
            var username = Request.Query["username"];

            var entity = _dal.Get(username, 0);

            switch (entity.accountStatus)
            {
                case CustomerStatus.PendingActivationWithoutPasswordChange:
                    if (entity.activationCode == activatioNCode)
                    {
                        entity.activationCode = null;
                        entity.accountStatus = sucessActivationStatus;
                        _dal.AddOrUpdate(entity, 0);
                        _myLogger.LogDebug(
                            $"++ Updatied entity AccountStatus from {CustomerStatus.PendingActivationWithoutPasswordChange} to {entity.accountStatus}");
                        return Redirect("/#!/signin/activationsucceeded");
                    }
                    return Content("Activation error");
                default:
                    throw new NotImplementedException();
            }
        }

        [HttpGet("empty")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual Customer GetEmpty()
        {
            var entity = new Customer
            {
                entityModel =
                    new List<object> {
                        new { name="firstName", type="string" },
                        new { name="lastName", type="string" },
                        new { name="email", type="string" },
                        new { name="passwordForTyping", type="string" },
                        new { name="adrLine1", type="string" },
                        new { name="adrLine2", type="string" },
                        new { name="adrPostalCode", type="string" },
                        new { name="adrCity", type="string" },
                        new { name="adrCountry", type="string" }
                    }
            };
            return entity;
        }

        [HttpGet("forbidden")]
        public IActionResult Forbidden() => Unauthorized();

        [HttpPost("activateasadmin")]
        [Authorize("Administrator")]
        public Customer ActivateAsAdmin([FromBody] dynamic values) => _dal.UpdateStatus(new Customer(values).id, CustomerStatus.Activated, _sessionHelperInstance.User.id);

        [HttpPost("disableasadmin")]
        [Authorize("Administrator")]
        public Customer DisableAsAdmin([FromBody] dynamic values) => _dal.UpdateStatus(new Customer(values).id, CustomerStatus.Disabled, _sessionHelperInstance.User.id);


        [AllowAnonymous]
        [HttpPost("GenerateToken")]
        public IActionResult GenerateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dal.Get(model.Email.ToString(), model.Password.ToString());

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                      new Claim(JwtRegisteredClaimNames.Sub, user.email),
                      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    if (!string.IsNullOrEmpty(user.rolesString))
                        foreach (var cur in user.rolesString.Split(',').Select(a => a.Trim()))
                            claims.Add(new Claim(ClaimTypes.Role, cur));

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                      _config["Tokens:Issuer"],
                      claims,
                      expires: DateTime.Now.AddMinutes(30),
                      signingCredentials: creds);

                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
            }

            return BadRequest("Could not create token");
        }

        public class LoginViewModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }


    }
}
