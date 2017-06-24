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
    public class CustomerDal : AbstractDal<Customer>
    {
        public override Customer Get(int id, int userId)
        {
            var entity = base.Get(id, userId);
            //entity.entityColumnsModel = ReflectionHelper.GetColumns(entity).ToDictionary(a => a.name, a => a);
            if (entity != null && entity.customerMainAdressId != null)
                entity.customerMainAdress = new PostalAddressDal().Get((int)entity.customerMainAdressId, userId);
            //entity.userRights = new CustomerHasModuleDal().ListFor(id, userId).ToList();
            //entity.checkBoxListSelectedValues = entity.userRights.Select(a => a.encodedName).ToList();

            return entity;
        }

        public override Customer Get(string username, int userId)
        {
            var sql = $"SELECT * FROM Customer WHERE email = '{username}' AND DeletionDate IS NULL";
            var item = QuerySql(sql, userId).Single();
            return item;
        }

        public Tuple<int, string> Get(string userName, string passsword)
        {

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var sql = $"SELECT id, email FROM Customer WHERE email='{userName}' AND password='{new EncryptionHelper().GetHash(passsword)}' AND AccountStatus='Activated' AND DeletionDate IS NULL";
                var entity = conn.Query(sql).SingleOrDefault();
                if (entity != null)
                    return new Tuple<int, string>(entity.id, entity.email);
                return null;
            }
        }
        
    }
}
