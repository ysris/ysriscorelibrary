using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;

namespace YsrisCoreLibrary.Controllers
{
    public class AbstractCustomerCompanyController<T, U> : AbstractController<T> where T : class, IAbstractEntity, ICustomerCompany, new() where U : class, ICustomer
    {
        protected readonly IStorageService _storage;
        protected readonly SessionHelperService<Customer> _session;
        protected readonly IHostingEnvironment _env;
        protected readonly StripeService _stripe;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<T> _log;

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="session"></param>
        /// <param name="storageService"></param>
        public AbstractCustomerCompanyController(
            DbContext context,
            SessionHelperService<Customer> session,
            IStorageService storageService,
            IHostingEnvironment env,
            StripeService stripe,
            IConfiguration configuration,
            ILogger<T> log
        ) : base(context)
        {
            _storage = storageService;
            _session = session;
            _env = env;
            _stripe = stripe;
            _configuration = configuration;
            _log = log;
        }
        #endregion

        /// <summary>
        /// Get connected user account action
        /// </summary>
        /// <returns>Customer</returns>
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies")]
        public virtual async Task<IActionResult> GetMe()
        {
            return Ok(await _getMe());
        }

        protected virtual async Task<T> _getMe()
        {
            if (_session.User == null)
                return null;
            var entity = await _context.Set<T>().FindAsync(_session.User.companyId);
            if ((entity).creatorCustomerId != null)
                entity.creatorustomer = await _context.Set<U>().FindAsync(entity.creatorCustomerId); //todo : move upper in

            if (entity != null)
                entity.pictureClientAccessor = $"/api/customercompany/avatar/{entity.id}";

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
            return await GetAvatar((int)_session.User.companyId);
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

            var entity = await _context.Set<CustomerCompany>().FindAsync(id);
            if (entity?.picture != null)
                path = entity.picture;

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
        /// Upload avatar of connected user
        /// </summary>
        /// <returns></returns>
        [HttpPost("avatar")]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "CompanyAdministrator")]
        public virtual async Task<CustomerCompany> UploadAvatar(IFormFile file)
        {
            _log.LogInformation($"+ UploadAvatar file={file}");

            var largePath = $"/companyavatars/large/{_session.User.companyId}.jpg";
            _storage.SavePictureTo(file, largePath, 300);

            var entity = await _context.Set<CustomerCompany>().FindAsync(_session.User.companyId);
            entity.picture = largePath;

            _context.Set<CustomerCompany>().Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }


        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "CompanyAdministrator")]
        public override Task<IActionResult> Patch([FromBody] T values)
        {
            var entity = _context.Set<T>().Find(_session.User.companyId);
            entity.SetFromValues(values);
            return base.Patch(entity);
        }

    }
}
