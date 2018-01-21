using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Dal
{
    public class CustomerHasModuleDal : AbstractDal<CustomerHasModule>
    {
        private IConfiguration _configuration;

        public CustomerHasModuleDal(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<CustomerHasModule> ListFor(int id, int userId)
        {
            var set = QuerySql($@"SELECT * FROM {_tableName} WHERE CustomerId='{id}' AND DeletionDate IS NULL;", userId).ToList();

            //var f = ReflectionHelper.GetAvailableMenuItems();

            set.ForEach(a =>
            {
                a.httpMethod = string.IsNullOrEmpty(a.httpMethod) ? "HttpGet" : a.httpMethod;
                //var entity = f.Where(b => b.Equals(a)).SingleOrDefault();                
            });

            return set;
        }
    }
}
