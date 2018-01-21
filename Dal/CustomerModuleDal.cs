using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Dal
{
    public class CustomerModuleDal : AbstractDal<object>
    {
        private IConfiguration _configuration;

        public CustomerModuleDal(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        //public IEnumerable<CustomerHasModule> ListModules(Customer user, int userId)
        //{
        //    var sql = $"SELECT * FROM CustomerHasModule WHERE CustomerId = '{user.id}' AND DeletionDate IS NULL";
        //    var set = QuerySql(sql, userId);
        //    return set;
        //}

        public void UpdateRightsFor(Customer customer, List<string> rawRolesString, int userId)
        {
            // 1. Remove actual rights
            var sql =
                $@"UPDATE CustomerHasModule 
                SET DeletionDate=GETDATE() 
                WHERE CustomerId = '{customer.id}' 
                AND DeletionDate IS NULL";
            ExecuteSql(sql, userId);

            //TODO : Transactionnal

            // 2. Recreate
            foreach (var cur in rawRolesString)
            {
                var exploded = cur.Split(':');
                var customerId = customer.id;
                string
                    area = exploded[0],
                    controller = exploded[1],
                    action = exploded[2],
                    httpmethod = exploded[3];

                sql =
                    $@"INSERT INTO CustomerHasModule 
                    (
                        CustomerId
                        , AreaName
                        , ControllerName
                        , ActionName
                        , HttpMethod
                        , CreationDate
                    ) 
                    VALUES 
                    (
                        {customerId}
                        , '{area}'
                        , '{controller}'
                        , '{action}'
                        , '{httpmethod}'
                        , GETDATE()
                    );";
                ExecuteSql(sql, userId);
            }
        }

    }
}
