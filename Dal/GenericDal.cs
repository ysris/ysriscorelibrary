using Microsoft.Extensions.Configuration;
using YsrisCoreLibrary.Dal;

namespace redpasta.Dal
{
    public class GenericDal<T> : AbstractDal<T> where T : class
    {
        private IConfiguration _configuration;

        public GenericDal(IConfiguration configuration) : base(configuration)
        {
            this._configuration = configuration;
        }

        
    }
}