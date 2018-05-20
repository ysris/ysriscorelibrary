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
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;
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
        #region Fields
        protected readonly DbContext _context;
        protected readonly IConfiguration _config;
        protected readonly MailHelperService _mail;
        protected readonly IStorageService _storage;
        protected readonly IHostingEnvironment _env;
        protected readonly EncryptionService _encryption;
        protected readonly SessionHelperService _session;
        protected readonly ILogger<AbstractCustomerController<T>> _log;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public AbstractCustomerController(
            DbContext context,
            IConfiguration config,
            MailHelperService mail,
            IHostingEnvironment env,
            IStorageService storage,
            EncryptionService encryption,
            SessionHelperService sessionHelper,
            ILogger<AbstractCustomerController<T>> log
            )
        {
            _log = log;
            _env = env;
            _mail = mail;
            _config = config;
            _context = context;
            _storage = storage;
            _encryption = encryption;
            _session = sessionHelper;
        }
        #endregion

        #region Anonymous Actions API Methods
        /// <summary>
        /// account creation action
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] T model)
        {
            _log.LogDebug($"CustomerController +Post");
            var entity = await _createAccount(model);
            _sendActivationEmail(entity);
            return Ok(entity);
        }

        /// <summary>
        /// Standard cookie login
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public virtual async Task<IActionResult> Login([FromBody] LoginViewModel model, IEnumerable<string> accountStatuses = null)
        {
            if (accountStatuses == null || !accountStatuses.Any())
                accountStatuses = new List<string> { CustomerStatus.Activated };

            var customer =
                _context.Set<T>()
                .SingleOrDefault(a =>
                    a.email == model.username
                    //&& a.password == _encryption.GetHash(model.password)
                    //&& accountStatuses.Contains(a.accountStatus)
                    //&& a.deletionDate == null
                );

            if (customer == null)
                throw new Exception("Unauthorized");

            await _signin(customer);

            return Ok(customer);
        }

        /// <summary>
        /// Password recover
        /// </summary>
        /// <param name="obj"></param>
        [AllowAnonymous]
        [HttpPost("recover")]
        public virtual async Task<IActionResult> Recover([FromBody]RecoverViewModel model)
        {
            if (model.email == null)
                throw new Exception("incorrect parameters specified");

            var entity = _context.Set<T>().Single(a => a.email == model.email);

            entity.activationCode = Guid.NewGuid().ToString();
            entity.recoverAskDate = DateTime.Now;
            entity.accountStatus = CustomerStatus.PendingActivationWithPasswordChange;

            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            _mail.SendMail(
                entity.email,
                subject: "Password recover",
                templateUri: _env.ContentRootPath + "\\Views\\Emails\\UserPasswordReset.cshtml",
                mailViewBag:
                new Dictionary<string, string>
                {
                    { "FirstName", entity.prettyName },
                    { "RecoverUrl", $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/#!/passwordrecover2/{entity.email}/{entity.activationCode}" },
                    { "AppName", _config.GetValue<string>("Data:AppName")},
                    { "LogoDefault", $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/assets/images/logo-default.png"},
                    { "PrimaryColor", _config.GetValue<string>("Data:PrimaryColor")}
                }
            );

            return Ok();
        }

        /// <summary>
        /// Password recover callback 
        /// </summary>
        /// <param name="obj"></param>
        [AllowAnonymous]
        [HttpPost("recover2")]
        public virtual async Task<IActionResult> Recover2([FromBody]RecoverViewModel model)
        {
            var entity = _context.Set<T>().Single(a => a.email == model.email);
            if (entity == null || entity.activationCode == null || entity.recoverAskDate == null)
                throw new Exception("BadRequest");

            if (model.activationCode == entity.activationCode && (DateTime.Now - (DateTime)entity.recoverAskDate).Minutes <= 10)
            {
                entity.activationCode = null;
                entity.recoverAskDate = null;
                entity.password = _encryption.GetHash(model.password);
                entity.accountStatus = CustomerStatus.Activated;

                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();

                _mail.SendMail(
                    entity.email,
                    subject: "Password recover successful",
                    templateUri: _env.ContentRootPath + "\\Views\\Emails\\UserPasswordResetConfirmation.cshtml",
                    mailViewBag: new Dictionary<string, string> {
                        { "FirstName", entity.prettyName },
                        { "AppName", _config.GetValue<string>("Data:AppName")},
                        { "LogoDefault", $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/assets/images/logo-default.png"},
                        { "PrimaryColor", _config.GetValue<string>("Data:PrimaryColor")}
                    }
                );
            }

            return Ok();
        }

        /// <summary>
        /// Activation invitation email validation processing
        /// </summary>
        /// <param name="mode">the viewmode</param>        
        /// <param name="sucessActivationStatus">Activation status to set after a correction activation (email+activationcode combination correct)</param>        
        /// <returns>The HTTP status only</returns>
        [AllowAnonymous]
        [HttpPost("activateinvitation")]
        public virtual async Task<IActionResult> ActivateInvitation([FromBody] RecoverViewModel model, string sucessActivationStatus = CustomerStatus.Activated)
        {
            _log.LogInformation($"+ ActivateInvitation for {model.email}");

            var entity = await _context.Set<T>().SingleOrDefaultAsync(a => a.email == model.email);
            if (entity == null || entity.activationCode == null || model.activationCode != entity.activationCode)
                return StatusCode(StatusCodes.Status400BadRequest);

            entity.activationCode = null;
            entity.recoverAskDate = null;
            entity.password = _encryption.GetHash(model.password);
            entity.accountStatus = sucessActivationStatus;

            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            _log.LogInformation($"- ActivateInvitation for {model.email}");
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sucessActivationStatus"></param>
        /// <returns></returns>
        [HttpGet("activate")]
        public virtual async Task<IActionResult> Activate(string sucessActivationStatus = CustomerStatus.Activated)
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
                        await _context.SaveChangesAsync();

                        _log.LogDebug(
                            $"++ Updatied entity AccountStatus from {CustomerStatus.PendingActivationWithoutPasswordChange} to {entity.accountStatus}");
                        return Redirect("/#!/signin/activationsucceeded");
                    }
                    return Content("Activation error");
                case CustomerStatus.PendingActivationWithPasswordChange:
                    throw new Exception("There is nothing to do here...");
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Generate JWT Token logic
        /// NOT Async by choice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("GenerateToken")]
        public IActionResult GenerateToken([FromBody] GenerateTokenLoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = _context.Set<T>().Single(a => a.email == model.Email && a.password == _encryption.GetHash(model.Password));

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
        #endregion

        #region Connected standard Customer Actions API Methods
        /// <summary>
        /// Standard cookie logout
        /// </summary>
        [HttpPost("logout")]
        public virtual async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        /// <summary>
        /// Get connected user account action
        /// </summary>
        /// <returns>Customer</returns>
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<T> GetMe()
        {
            if (_session.User == null)
                return null;
            var entity = await _context.Set<T>().FindAsync(_session.User.id);

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
        public virtual async Task<IActionResult> GetAvatar()
        {
            if (_session.User != null)
            {
                var entity = await _context.Set<T>().FindAsync(_session.User.id);
                var smallUri = entity.picture;

                if (smallUri == null)
                {
                    var path = Path.Combine(_env.WebRootPath, "bobos_components/assets/images/profile-placeholder.png");
                    return File(System.IO.File.ReadAllBytes(path), "image/png");
                }
                var result = _storage.GetFileContent(smallUri)?.Result?.ToArray();
                if (result == null)
                    return File(System.IO.File.ReadAllBytes(Path.Combine(_env.WebRootPath, "bobos_components/assets/images/profile-placeholder.png")), "image/png");
                return File(result, "image/jpeg");
            }
            return File(System.IO.File.ReadAllBytes(Path.Combine(_env.WebRootPath, "bobos_components/assets/images/profile-placeholder.png")), "image/png");
        }

        /// <summary>
        /// Get any user avatar file content action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("avatar/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<IActionResult> GetAvatar(int id)
        {
            var path = Path.Combine(_env.WebRootPath, "bobos_components/assets/images/profile-placeholder.png");

            var entity = await _context.Set<T>().FindAsync(id);

            if (entity.picture == null)
                return File(System.IO.File.ReadAllBytes(path), "image/png");

            try
            {
                var result = _storage.GetFileContent(entity.picture)?.Result?.ToArray();
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
        /// account update action
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("update")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<T> Update([FromBody] T model)
        {
            var entity = await _context.Set<T>().FindAsync(_session.User.id);
            entity.SetFromValues(model);

            if (entity.id != _session.User.id)
                throw new Exception("Unauthorized");

            if (model.rawPasswordConfirm != null && model.passwordForTyping != null)
                if (!string.IsNullOrEmpty(model.rawPasswordConfirm.ToString()) &&
                    !string.IsNullOrEmpty(model.passwordForTyping.ToString()))
                {
                    if (model.rawPasswordConfirm.ToString() != model.passwordForTyping.ToString())
                        throw new Exception("Password confirmation mismatch");
                    entity.password = _encryption.GetHash(model.passwordForTyping.ToString());
                }

            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            _session.HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(entity));

            return entity;
        }

        /// <summary>
        /// Upload avatar of connected user
        /// </summary>
        /// <returns></returns>
        [HttpPost("avatar")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<T> UploadAvatar(IFormFile file)
        {
            _log.LogInformation($"+ UploadAvatar file={file}");

            var largePath = $"/avatars/large/{_session.User.id}.jpg";
            _storage.SavePictureTo(file, largePath, 300);

            var entity = await _context.Set<T>().FindAsync(_session.User.id);
            entity.picture = largePath;

            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Delete user account
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async void Delete()
        {
            var entity = _context.Set<T>().Find(_session.User.id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Administrator Actions API Methods
        [HttpPost("invite")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")] //todo : the default policy is probably incorrect
        public virtual T Invite([FromBody] InviteCustomerViewModel model)
        {
            _log.LogInformation($"AbstractCustomerController +Invite");

            if (_context.Set<T>().Where(a => a.email == model.entity.email).Any())
                throw new Exception("Already assigned email");

            model.entity.activationCode = Guid.NewGuid().ToString();
            model.entity.customerType = Role.User;
            model.entity.createdAt = DateTime.Now;
            model.entity.accountStatus = CustomerStatus.PendingActivationWithPasswordChange;
            model.entity.rolesString = string.Join(",", new List<string>() { Role.User });

            _context.Set<T>().Add(model.entity);
            _context.SaveChanges();

            // 4. User notification
            if (model.boolSendEmail)
            {
                _log.LogDebug($"+++User notification (mail)");
                _mail.SendMail(
                    model.entity.email,
                    subject: $"You have been invited to join {HttpContext.Request.Host}",
                    templateUri: _env.ContentRootPath + "/Views/Emails/CustomerInvitation.cshtml",
                    mailViewBag:
                    new Dictionary<string, string>
                    {
                        { "FirstName", model.entity.prettyName },
                        { "ActivationUrl",$"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/#!/activateinvitation/{model.entity.email}/{model.entity.activationCode}" },
                        { "AppName", _config.GetValue<string>("Data:AppName") },
                        { "LogoDefault",$"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/assets/images/logo-default.png" },
                        { "PrimaryColor", _config.GetValue<string>("Data:PrimaryColor") }
                    }
                );
            }
            _log.LogInformation($"AbstractCustomerController -Invite");
            return model.entity;
        }

        /// <summary>
        /// account update action
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("updateasadmin")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public virtual T UpdateAsAdmin([FromBody] T model)
        {
            var entity = _context.Set<T>().Find(_session.User.id);
            entity.SetFromValues(model);

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            _session.HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(entity));

            return entity;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("activateasadmin")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public T ActivateAsAdmin([FromBody] T model)
        {
            var entity = _context.Set<T>().Find(model.id);
            entity.accountStatus = CustomerStatus.Activated;

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            return entity;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost("disableasadmin")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public T DisableAsAdmin([FromBody] T model)
        {
            var entity = _context.Set<T>().Find(model.id);
            entity.accountStatus = CustomerStatus.Disabled;

            _context.Set<T>().Update(entity);
            _context.SaveChanges();

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("grant")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public virtual async Task<IActionResult> GrantUserRole([FromBody] UserRoleAttributionViewModel model)
        {
            _log.LogInformation($"AbstractCustomerController +GrantUserRole");
            var entity = _context.Set<T>().Find(model.entity.id);
            if (!entity.roles.Contains(model.role))
            {
                var bfr = entity.roles;
                bfr.Add(model.role);
                entity.rolesString = string.Join(", ", bfr);
            }

            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            _log.LogInformation($"AbstractCustomerController -GrantUserRole");
            return Ok(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("revoke")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public virtual async Task<IActionResult> RevokeUserRole([FromBody] UserRoleAttributionViewModel model)
        {
            var entity = _context.Set<T>().Find(model.entity.id);

            if (entity.id == _session.User.id)
                throw new Exception("Cannot revoke Admin role for the current connected user");

            if (entity.roles.Contains(model.role))
            {
                var bfr = entity.roles;
                bfr.Remove(model.role);
                entity.rolesString = string.Join(',', bfr);
            }

            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            return Ok(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public virtual async Task<IActionResult> List()
        {
            var set = await _context.Set<T>().ToListAsync();
            return Ok(set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("projection")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "Administrator")]
        public virtual async Task<IActionResult> Projection()
        {
            var set = await _context.Set<T>().ToListAsync();
            return Ok(new
            {
                entitylist = set,
                entityCount = set.Count()
            });
        }
        #endregion

        #region Protected Logic Methods
        /// <summary>
        /// Signin logic extraction to simplify logic redefinition or external call
        /// </summary>
        /// <param name="customer"></param>
        protected virtual async Task<T> _signin(T customer)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, customer.email) };
            if (!string.IsNullOrEmpty(customer.rolesString))
                foreach (var cur in customer.rolesString.Split(',').Select(a => a.Trim()))
                    claims.Add(new Claim(ClaimTypes.Role, cur));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            HttpContext.Session.SetString("UserEntity", (string)JsonConvert.SerializeObject(customer));

            return customer;
        }

        /// <summary>
        /// Create account extraction to simplify logic redefinition or external call
        /// </summary>
        protected virtual async Task<T> _createAccount(T model)
        {
            var test = _context.Set<T>().Where(a => a.email == model.email);
            if (test.Any())
                throw new Exception("Already assigned email");

            var rolesList = new List<string>();
            var customerType = Role.User;

            if (model.customerType != null)
            {
                switch (model.customerType)
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
                email = model.email,
                firstName = model.firstName,
                lastName = model.lastName,
                customerType = customerType,
                createdAt = DateTime.Now,
                rolesString = string.Join(",", rolesList.Select(a => a.ToString())),
                id = 0
            };
            if (model.passwordForTyping == null)
                throw new Exception("Password is null");
            entity.password = _encryption.GetHash(model.passwordForTyping.ToString()); //keep here for avoiding the model binding
            entity.companyId = 0;
            entity.activationCode = Guid.NewGuid().ToString();
            entity.accountStatus = CustomerStatus.PendingActivationWithoutPasswordChange;

            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// send activation email extraction to simplify redefinition or external call
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void _sendActivationEmail(T entity)
        {
            // 4. User notification
            _log.LogDebug($"+++User notification (mail)");
            _mail.SendMail(
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
        #endregion

        // TODO : THIS IS BAD AND YOU SHOULD FEEL BAD ABOUT IT //

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
        public class InviteCustomerViewModel
        {
            public T entity { get; set; }
            public bool boolSendEmail { get; set; }
        }
        public class UserRoleAttributionViewModel
        {
            public T entity { get; set; }
            public string role { get; set; }
        }
    }
}
