using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;

namespace YsrisCoreLibrary.Controllers
{
    public abstract class AbstractConversationMessageController : AbstractController<ConversationMessage>
    {
        protected SessionHelperService<Customer> _session;
        protected ILogger _logger;

        public AbstractConversationMessageController(DbContext context) : base(context)
        {
        }

        /// <summary>
        /// List messages for a specific dest customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("getforcustomer/{customerId}")]
        public IQueryable<ConversationMessage> GetForDestCustomer(int customerId)
        {
            var collection =
                (
                    from a in _context.Set<ConversationMessage>()
                    where a.authorId == _session.User.id || a.destId == _session.User.id
                    where a.destId == customerId || a.authorId == customerId
                    orderby a.creationDate
                    select a
                )
                .ToList();

            foreach (var item in collection)
            {
                var idx = collection.IndexOf(item);
                var precItem = idx > 0 ? collection[idx - 1] : null;
                item.isDaySwitch = item.creationDate.Date != precItem?.creationDate.Date;

                item.isConnectedUserAuthor = item.authorId == _session.User.id;
            }

            foreach (var item in collection.Where(a => a.destId == _session.User.id))
                item.isReadByDest = true;

            _context.UpdateRange(collection);
            _context.SaveChanges();

            return collection.AsQueryable();
        }

        /// <summary>
        /// Post a message
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        //[HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer, Cookies", Policy = "All")]
        public override async Task<IActionResult> Post([FromBody] ConversationMessage entity)
        {
            _logger.LogDebug("conversationmessage +Post");

            entity.creationDate = DateTime.Now;
            entity.authorId = _session.User.id;
            entity.isReadByDest = false;

            _context.Set<ConversationMessage>().Add(entity);
            await _context.SaveChangesAsync();

            _logger.LogDebug("conversationmessage -Post");
            return CreatedAtAction("Get", new { id = entity.id }, entity);
        }
    }
}
