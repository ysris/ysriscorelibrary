using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Helpers;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Dal
{
    public class CustomerDal : AbstractCustomerDal
    {
        public override Customer Get(string userName, string passsword, string tableName = "Customer")
        {
            using (var conn = _getConnection(ConfigurationHelper.ConnectionString))
            {
                conn.Open();
                var sql = $"SELECT * FROM {tableName} WHERE email='{userName}' AND password='{new EncryptionHelper().GetHash(passsword)}' AND AccountStatus='Activated' AND DeletionDate IS NULL";
                var entity = conn.Query<Customer>(sql).SingleOrDefault();                
                return entity;
            }
        }

        public Customer GetByApiKey(string apiKey, string tableName = "Customer")
        {
            using (var conn = _getConnection(ConfigurationHelper.ConnectionString))
            {
                conn.Open();
                var sql = $"SELECT * FROM {tableName} WHERE apiKey='{apiKey}' AND AccountStatus='Activated' AND DeletionDate IS NULL";
                var entity = conn.Query<Customer>(sql).SingleOrDefault();
                return entity;
            }
        }
    }
}
