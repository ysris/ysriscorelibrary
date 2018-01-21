using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Dal;
using YsrisCoreLibrary.Helpers;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Enums;
using Microsoft.Extensions.Configuration;

namespace YsrisCoreLibrary.Dal
{
    public abstract class AbstractCustomerDal : AbstractDal<Customer>
    {
        public AbstractCustomerDal(IConfiguration configuration) : base(configuration)
        {

        }

        public override Customer Get(string username, int userId, string tableName = "Customer")
        {
            var sql = $"SELECT * FROM {tableName} WHERE email = '{username}' AND DeletionDate IS NULL";
            var item = QuerySql(sql, userId).Single();
            return item;
        }

        public abstract Customer Get(string userName, string passsword, string tableName = "Customer");

        public Customer UpdateStatus(int customerId, string status, int connectedUserId)
        {
            var entity = Get(customerId, connectedUserId);
            entity.accountStatus = status;
            entity = AddOrUpdate(entity, connectedUserId) as Customer;
            return entity;
        }
    }
}
