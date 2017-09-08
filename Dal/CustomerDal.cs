using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Helpers;

namespace YsrisCoreLibrary.Dal
{
    public class CustomerDal : AbstractCustomerDal
    {
        

        public override Tuple<int, string> Get(string userName, string passsword, string tableName = "Customer")
        {
            using (var conn = _getConnection(ConfigurationHelper.ConnectionString))
            {
                conn.Open();
                var sql = $"SELECT id, email FROM {tableName} WHERE email='{userName}' AND password='{new EncryptionHelper().GetHash(passsword)}' AND AccountStatus='Activated' AND DeletionDate IS NULL";
                var entity = conn.Query(sql).SingleOrDefault();
                if (entity != null)
                    return new Tuple<int, string>(entity.id, entity.email);
                return null;
            }
        }
    }
}
