//using Dapper;
//using ImageSharp;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using YsrisCoreLibrary.Abstract;
//using YsrisCoreLibrary.Helpers;
//using YsrisCoreLibrary.Models;
//using YsrisCoreLibrary.Services;

//namespace YsrisCoreLibrary.Dal
//{
//    public class CustomerDal : AbstractDal<Customer>
//    {
//        public IConfiguration Configuration { get; }
//        protected EncryptionService _encryptionHelper;

//        public CustomerDal(IConfiguration configuration, EncryptionService encryptionHelper) : base(configuration)
//        {
//            Configuration = configuration;
//            _encryptionHelper = encryptionHelper;
//        }

//        public override Customer Get(string username, int userId, string tableName = "Customer")
//        {
//            var sql = $"SELECT * FROM {tableName} WHERE email = '{username}' AND DeletionDate IS NULL";
//            var item = QuerySql(sql, userId).Single();
//            return item;
//        }

//        public virtual Customer Get(string userName, string passsword, string tableName = "Customer")
//        {
//            using (var conn = _getConnection(Configuration.GetValue<string>("Data:DefaultConnection:ConnectionString")))
//            {
//                conn.Open();
//                var sql = $"SELECT * FROM {tableName} WHERE email='{userName}' AND password='{_encryptionHelper.GetHash(passsword)}' AND AccountStatus='Activated' AND DeletionDate IS NULL";
//                var entity = conn.Query<Customer>(sql).SingleOrDefault();
//                return entity;
//            }
//        }


//        public Customer UpdateStatus(int customerId, string status, int connectedUserId)
//        {
//            var entity = Get(customerId, connectedUserId);
//            entity.accountStatus = status;
//            entity = AddOrUpdate(entity, connectedUserId) as Customer;
//            return entity;
//        }

//        public Customer GetByApiKey(string apiKey, string tableName = "Customer")
//        {
//            using (var conn = _getConnection(Configuration.GetValue<string>("Data:DefaultConnection:ConnectionString")))
//            {
//                conn.Open();
//                var sql = $"SELECT * FROM {tableName} WHERE apiKey='{apiKey}' AND AccountStatus='Activated' AND DeletionDate IS NULL";
//                var entity = conn.Query<Customer>(sql).SingleOrDefault();
//                return entity;
//            }
//        }


//    }
//}
