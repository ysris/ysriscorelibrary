using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Dal
{
    public class PostalAddressDal : AbstractDal<PostalAddress>
    {
        private IConfiguration _configuration;

        public PostalAddressDal(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;

        }
    }
}
