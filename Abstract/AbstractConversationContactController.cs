using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;

namespace YsrisCoreLibrary.Abstract
{
    public abstract class AbstractConversationContactController : AbstractController<Customer>
    {
        public AbstractConversationContactController(DbContext context) : base(context)
        {
        }

        private SessionHelperService _session;
        private ILogger<AbstractConversationContactController> _logger;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AbstractConversationContactController(
            DbContext context,
            SessionHelperService session,
            ILogger<AbstractConversationContactController> logger
        ) : base(context)
        {
            _session = session;
            _logger = logger;
        }

        public override IQueryable<Customer> Get()
        {
            var contactIds =
                from a in _context.Set<ConversationMessage>()
                where a.authorId == _session.User.id || a.destId == _session.User.id
                select a.authorId == _session.User.id ? a.destId : a.authorId
                ;

            var customers =
                _context.Set<Customer>().Where(a => contactIds.Contains(a.id));

            return customers;
        }
    }
}
