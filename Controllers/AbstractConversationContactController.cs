using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;

namespace YsrisCoreLibrary.Controllers
{
    /// <summary>
    /// Controller abstraction for conversation contact related
    /// </summary>
    public abstract class AbstractConversationContactController : AbstractController<Customer>
    {
        protected SessionHelperService _session;
        protected ILogger<AbstractConversationContactController> _logger;

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

        /// <summary>
        /// Default way of getting contacts of a customer (by using distinct values of already started discussions)
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Customer> Get()
        {
            var set = _context.Set<ConversationMessage>();
            var contactIds =
                from a in set
                where a.authorId == _session.User.id || a.destId == _session.User.id
                select a.authorId == _session.User.id ? a.destId : a.authorId
                ;

            var customers =
                _context.Set<Customer>().Where(a => contactIds.Contains(a.id));

            return customers;
        }
    }
}
