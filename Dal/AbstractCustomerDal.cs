using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;
using YsrisCoreLibrary.Dal;
using YsrisCoreLibrary.Helpers;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Dal
{
    public abstract class AbstractCustomerDal : AbstractDal<Customer>
    {

        

        public override Customer Get(string username, int userId, string tableName = "Customer")
        {
            var sql = $"SELECT * FROM {tableName} WHERE email = '{username}' AND DeletionDate IS NULL";
            var item = QuerySql(sql, userId).Single();
            return item;
        }

        public abstract Tuple<int, string> Get(string userName, string passsword, string tableName = "Customer");

    }
}
