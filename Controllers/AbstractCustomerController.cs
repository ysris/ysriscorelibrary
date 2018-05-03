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
using YsrisCoreLibrary.Helpers;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;
using YsrisCoreLibrary.Abstract;
using ysriscorelibrary.Interfaces;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace YsrisCoreLibrary.Controllers
{
    /// <summary>
    /// Default customer management
    /// </summary>
    public abstract class AbstractCustomerController<T> : Controller where T : class, ICustomer, new()
    {
        protected readonly MailHelperService _mailHelperService;
        protected readonly IHostingEnvironment _env;
        protected readonly ILogger<AbstractCustomerController<T>> _myLogger;
        protected readonly SessionHelperService _session;
        protected readonly IStorageService _storageService;
        protected readonly IConfiguration _config;
        protected readonly EncryptionService _encryptionHelper;
        protected readonly DbContext _context;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sessionHelper"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        /// <param name="mailHelperService"></param>
        public AbstractCustomerController(
            SessionHelperService sessionHelper,
            ILogger<AbstractCustomerController<T>> logger,
            IHostingEnvironment env,
            MailHelperService mailHelperService,
            IStorageService storageService,
            IConfiguration config,
            EncryptionService encryptionHelper,
            DbContext context)
        {
            _session = sessionHelper;
            _myLogger = logger;
            _env = env;
            _mailHelperService = mailHelperService;
            _storageService = storageService;
            //_dal = dal;
            _config = config;
            _encryptionHelper = encryptionHelper;
            _context = context;
        }

        /// <summary>
        /// Standard cookie login
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public virtual LoginCustomerEntity Login([FromBody] LoginViewModel model, IEnumerable<string> accountStatuses = null)
        {
            if (accountStatuses == null || !accountStatuses.Any())
                accountStatuses = new List<string> { CustomerStatus.Activated };

            var customer =
                _context.Set<Customer>()
                .SingleOrDefault(a =>
                    a.email == model.username
                    && a.password == _encryptionHelper.GetHash(model.password)
                    && accountStatuses.Contains(a.accountStatus)
                    && a.deletionDate == null
                );

            if (customer == null)
                throw new Exception("Unknown User");

            _signin(customer);

            return new LoginCustomerEntity { customer = customer };
        }

        /// <summary>
        /// Standard cookie logout
        /// </summary>
        [HttpPost("logout")]
        public virtual async void Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Password recover
        /// </summary>
        /// <param name="obj"></param>
        [AllowAnonymous]
        [HttpPost("recover")]
        public virtual void Recover([FromBody]RecoverViewModel obj)
        {
            if (obj.email == null)
                throw new Exception("incorrect parameters specified");

            var entity = _context.Set<T>().Single(a => a.email == obj.email);

            entity.activationCode = Guid.NewGuid().ToString();
            entity.recoverAskDate = DateTime.Now;
            entity.accountStatus = CustomerStatus.PendingActivationWithPasswordChange;

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            _mailHelperService.SendMail(
                entity.email,
                subject: "Password recover",
                templateUri: _env.ContentRootPath + "\\Views\\Emails\\UserPasswordReset.cshtml",
                mailViewBag:
                new Dictionary<string, string>
                {
                    { "UserFirstName", entity.firstName },
                    { "RecoverUrl", $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/#!/passwordrecover2/{entity.email}/{entity.activationCode}" }
                }
            );
        }

        /// <summary>
        /// Password recover callback 
        /// </summary>
        /// <param name="obj"></param>
        [AllowAnonymous]
        [HttpPost("recover2")]
        public virtual void Recover2([FromBody]RecoverViewModel obj)
        {
            var entity = _context.Set<T>().Single(a => a.email == obj.email);
            if (entity == null || entity.activationCode == null || entity.recoverAskDate == null)
                throw new Exception("BadRequest");

            if (obj.activationCode == entity.activationCode && (DateTime.Now - (DateTime)entity.recoverAskDate).Minutes <= 10)
            {
                entity.activationCode = null;
                entity.recoverAskDate = null;
                entity.password = _encryptionHelper.GetHash(obj.password);
                entity.accountStatus = CustomerStatus.Activated;

                _context.Set<T>().Update(entity);
                _context.SaveChanges();

                _mailHelperService.SendMail(
                    entity.email,
                    subject: "Password recover",
                    templateUri: _env.ContentRootPath + "\\Views\\Emails\\UserPasswordResetConfirmation.cshtml",
                    mailViewBag: new Dictionary<string, string> { { "UserFirstName", entity.firstName } }
                );
            }
        }

        /// <summary>
        /// Invitation (customer account created by a site administrator) activation
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("activateinvitation")]
        public virtual IActionResult ActivateInvitation()
        {
            var username = Request.Query["username"];
            var activatioNCode = Request.Query["activationCode"];
            return Redirect($"/#!/activateinvitation/{username}/{activatioNCode}");
        }

        [AllowAnonymous]
        [HttpPost("activateinvitation")]
        public virtual IActionResult ActivateInvitation2([FromBody] RecoverViewModel obj)
        {
            var entity = _context.Set<T>().Single(a => a.email == obj.email);
            if (entity == null || entity.activationCode == null)
                throw new Exception("BadRequest");

            if (obj.activationCode == entity.activationCode)
            {
                entity.activationCode = null;
                entity.recoverAskDate = null;
                entity.password = _encryptionHelper.GetHash(obj.password);
                entity.accountStatus = CustomerStatus.Activated;

                _context.Set<T>().Update(entity);
                _context.SaveChanges();

                _mailHelperService.SendMail(
                    entity.email,
                    subject: "Password recover",
                    templateUri: _env.ContentRootPath + "\\Views\\Emails\\UserPasswordResetConfirmation.cshtml",
                    mailViewBag: new Dictionary<string, string> { { "UserFirstName", entity.firstName } }
                );
            }

            return Ok(new { });
        }

        [HttpPost("invite")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")] //todo : the default policy is probably incorrect
        public virtual T Invite([FromBody] InviteCustomerViewModel obj)
        {
            _myLogger.LogDebug($"CustomerController +Post");

            var test = _context.Set<T>().Where(a => a.email == obj.email);
            if (test.Any())
                throw new Exception("Already assigned email");

            var entity = new T()
            {
                email = obj.email,
                activationCode = Guid.NewGuid().ToString(),
                customerType = Role.User,
                createdAt = DateTime.Now,
                accountStatus = CustomerStatus.PendingActivationWithPasswordChange,
                rolesString = string.Join(",", new List<string>() { Role.User }),
            };

            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            // 4. User notification
            if (obj.boolSendEmail)
            {
                _myLogger.LogDebug($"+++User notification (mail)");
                _mailHelperService.SendMail(
                    entity.email,
                    subject: $"You have been invited to join {HttpContext.Request.Host}",
                    templateUri: _env.ContentRootPath + "/Views/Emails/CustomerInvitation.cshtml",
                    mailViewBag:
                    new Dictionary<string, string>
                    {
                    {"FirstName", entity.firstName},
                    {
                        "ActivationUrl",
                        $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}:{HttpContext.Connection.LocalPort}/api/customer/activateinvitation?username={entity.email}&activationCode={entity.activationCode}"
                    }
                    }
                );
            }

            return entity;
        }

        /// <summary>
        /// account creation action
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public virtual T Post([FromBody] T values)
        {
            _myLogger.LogDebug($"CustomerController +Post");

            var entity = _createAccount(values);
            entity.activationCode = Guid.NewGuid().ToString();
            entity.accountStatus = CustomerStatus.PendingActivationWithoutPasswordChange;

            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            _sendActivationEmail(entity);

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
            _myLogger.LogInformation($"+ UploadAvatar file={file}");

            var largePath = $"/avatars/large/{_session.User.id}.jpg";
            _storageService.SavePictureTo(file, largePath, 300);

            var entity = _context.Set<T>().Find(_session.User.id);
            entity.picture = largePath;

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            return entity;
        }

        /// <summary>
        /// account update action
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("updateasadmin")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public virtual T UpdateAsAdmin([FromBody] T values)
        {
            var entity = _context.Set<T>().Find(_session.User.id);
            entity.SetFromValues(values);

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            _session.HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(entity));

            return entity;
        }


        /// <summary>
        /// account update action
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("update")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual T Update([FromBody] T values)
        {
            var entity = _context.Set<T>().Find(_session.User.id);
            entity.SetFromValues(values);

            if (entity.id != _session.User.id)
                throw new Exception("Unauthorized");

            if (values.rawPasswordConfirm != null && values.passwordForTyping != null)
                if (!string.IsNullOrEmpty(values.rawPasswordConfirm.ToString()) &&
                    !string.IsNullOrEmpty(values.passwordForTyping.ToString()))
                {
                    if (values.rawPasswordConfirm.ToString() != values.passwordForTyping.ToString())
                        throw new Exception("Password confirmation mismatch");
                    entity.password = _encryptionHelper.GetHash(values.passwordForTyping.ToString());
                }

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            _session.HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(entity));

            return entity;
        }

        /// <summary>
        /// Get connected user account action
        /// </summary>
        /// <returns>Customer</returns>
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual T GetMe()
        {
            if (_session.User == null)
                return null;
            var entity = _context.Set<T>().Find(_session.User.id);

            if (entity != null)
                entity.pictureClientAccessor = $"/api/customer/avatar/{entity.id}";

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
            var entity = _context.Set<T>().Find(_session.User.id);
            if (entity != null)
                entity.pictureClientAccessor = $"/api/customer/avatar/{entity.id}";
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
            if (_session.User != null)
            {
                var entity = _context.Set<T>().Find(_session.User.id);
                var smallUri = entity.picture;

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
            var path = Path.Combine(_env.WebRootPath, "bobos_components/assets/images/profile-placeholder.png");

            var entity = _context.Set<T>().Find(id);

            if (entity.picture == null)
                return File(System.IO.File.ReadAllBytes(path), "image/png");

            try
            {
                var result = _storageService.GetFileContent(entity.picture)?.Result?.ToArray();
                if (result == null)
                    return File(System.IO.File.ReadAllBytes(path), "image/png");
                return File(result, "image/jpeg");
            }
            catch
            {
                return File(System.IO.File.ReadAllBytes(path), "image/png");
            }
        }

        /// <summary>
        /// Delete user account
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public void Delete()
        {
            var entity = _context.Set<T>().Find(_session.User.id);
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete user account
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public void DeleteAsAdmin(int id)
        {
            var entity = _context.Set<T>().Find(_session.User.id);
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }


        /// Now, ovverride of activate method is mandatory to simplify modification : email activation callback
        public virtual IActionResult Activate(string sucessActivationStatus = CustomerStatus.Activated)
        {
            var activatioNCode = Request.Query["activationCode"];
            var username = Request.Query["username"];

            var entity = _context.Set<T>().Single(a => a.email == username);

            switch (entity.accountStatus)
            {
                case CustomerStatus.PendingActivationWithoutPasswordChange:
                    if (entity.activationCode == activatioNCode)
                    {
                        entity.activationCode = null;
                        entity.accountStatus = sucessActivationStatus;

                        _context.Set<T>().Update(entity);
                        _context.SaveChanges();

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
            var entity = new Customer { };
            return entity;
        }

        [HttpGet("forbidden")]
        public IActionResult Forbidden() => Unauthorized();

        [HttpPost("activateasadmin")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public T ActivateAsAdmin([FromBody] T values)
        {
            var entity = _context.Set<T>().Find(values.id);
            entity.accountStatus = CustomerStatus.Activated;

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            return entity;

        }

        [HttpPost("disableasadmin")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public Customer DisableAsAdmin([FromBody] dynamic values)
        {
            var entity = _context.Set<T>().Find(values.id);
            entity.accountStatus = CustomerStatus.Disabled;

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            return entity;
        }


        [AllowAnonymous]
        [HttpPost("GenerateToken")]
        public IActionResult GenerateToken([FromBody] GenerateTokenLoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = _context.Set<T>().Single(a => a.email == model.Email && a.password == _encryptionHelper.GetHash(model.Password));

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


        protected void _signin(Customer customer)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, customer.email) };
            if (!string.IsNullOrEmpty(customer.rolesString))
                foreach (var cur in customer.rolesString.Split(',').Select(a => a.Trim()))
                    claims.Add(new Claim(ClaimTypes.Role, cur));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(customer));

        }

        protected T _createAccount(T values)
        {
            var test = _context.Set<T>().Where(a => a.email == values.email);
            if (test.Any())
                throw new Exception("Already assigned email");

            var rolesList = new List<string>();
            var customerType = Role.User;

            if (values.customerType != null)
            {
                switch (values.customerType)
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


            var entity = new T
            {
                email = values.email,
                firstName = values.firstName,
                lastName = values.lastName,
                customerType = customerType,
                createdAt = DateTime.Now,
                rolesString = string.Join(",", rolesList.Select(a => a.ToString())),
                id = 0
            };
            if (values.passwordForTyping == null)
                throw new Exception("Password is null");
            entity.password = _encryptionHelper.GetHash(values.passwordForTyping.ToString()); //keep here for avoiding the model binding
            entity.companyId = 0;



            return entity;
        }

        protected void _sendActivationEmail(T entity)
        {
            // 4. User notification
            _myLogger.LogDebug($"+++User notification (mail)");
            _mailHelperService.SendMail(
                entity.email,
                subject: "Confirm account creation",
                templateUri: _env.ContentRootPath + "/Views/Emails/SignUpConfirmation.cshtml",
                mailViewBag:
                new Dictionary<string, string>
                {
                    {"FirstName", entity.prettyName},
                    {
                        "ActivationUrl",
                        $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/customer/activate?username={entity.email}&activationCode={entity.activationCode}"
                    },
                    {"AppName", _config.GetValue<string>("Data:AppName")},
                    {
                        "LogoDefault",
                        $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/assets/images/logo-default.png"
                    },
                    {"PrimaryColor", _config.GetValue<string>("Data:PrimaryColor")}
                }
            );
        }

        public class AccountCreationViewModel
        {
            public string UserType { get; set; }
            public string email { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string passwordForTyping { get; set; }

        }

        public class RecoverViewModel
        {
            public string email { get; set; }
            public string activationCode { get; set; }
            public string password { get; set; }
        }

        public class LoginViewModel
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public class GenerateTokenLoginViewModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginCustomerEntity
        {
            public Customer customer { get; set; }
        }

        public class InviteCustomerViewModel
        {
            public string email { get; set; }
            public bool boolSendEmail { get; set; }
        }
    }
}
