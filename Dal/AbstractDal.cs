using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using YsrisCoreLibrary.Extensions;
using YsrisCoreLibrary.Helpers;

namespace YsrisCoreLibrary.Dal
{
    /// <summary>
    /// Default data access layer abstraction :
    /// - SQL mapping with local entities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractDal<T> where T : class
    {
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// From T, we get the name of the SQL table (table name == entity name)
        /// </summary>
        protected virtual string _tableName { get; }

        /// <summary>
        /// The connection string used in this context
        /// </summary>
        public virtual string ConnectionString { get; }

        public IDbConnection _getConnection(string connString) =>
            Configuration.GetValue<string>("Data:DefaultConnection:ConnectionType") == "MySql"
            ? (IDbConnection)new MySqlConnection(connString)
            : (IDbConnection)new SqlConnection(connString);

        /// <summary>
        /// Formatter to adapt a property name to the correct "SQL typing"
        /// </summary>
        private Func<object, string> formatter = a =>
            a == null ? "null"
            : a is string ? $"'{a.ToString()}'"
            : (a is DateTime || a is DateTime?) ? $"'{((DateTime)a).ToString("yyyy-MM-dd HH:mm:ss")}'"
            : a is int || a is decimal || a is decimal? || a is int? || a is double || a is float ? "'" + a.ToString().Replace(",", ".") + "'"
            : a.GetType().GetTypeInfo().BaseType == typeof(Enum) ? $"'{a.ToString()}'"
            : a.ToString();

        /// <summary>
        /// Default constructor
        /// </summary>
        public AbstractDal(IConfiguration configuration)
        {
            this.Configuration = configuration;
            _tableName = typeof(T).Name;
            ConnectionString = Configuration.GetValue<string>("Data:DefaultConnection:ConnectionString");
        }

        #region Querying

        public IEnumerable<T> QuerySql(string sqlStatement, int userId, string connectionString = null) => QuerySql<T>(sqlStatement, userId, connectionString);

        /// <summary>
        /// Execute an SQL query and gives the result back
        /// </summary>
        /// <typeparam name="Y">Type of the entity that should be retrieved (automatic casting)</typeparam>
        /// <param name="sql">Select SQL Query</param>
        /// <param name="userId">current connected user id</param>
        /// <param name="connectionString">connection string that should be used instead of the default context connection string</param>
        /// <returns></returns>
        public IEnumerable<Y> QuerySql<Y>(string sql, int userId, string connectionString = null) where Y : class
        {
            try
            {
                using (var conn = _getConnection(connectionString ?? ConnectionString))
                {
                    conn.Open();
                    return conn.Query<Y>(sql, null);
                }
            }
            catch (SqlException)
            {
                var sqlTxt = $"SQL Query error, statement:'{sql}'";
                //LogHelper.Error<Y>(sqlTxt);
                throw new Exception(sqlTxt);
            }
        }
        public void ExecuteSql(string sql, int userId, string connectionString = null)
        {
            try
            {
                using (var conn = _getConnection(connectionString ?? ConnectionString))
                {
                    conn.Open();
                    conn.Execute(sql);
                }
            }
            catch (SqlException)
            {
                var sqlTxt = $"SQL Query error, statement:'{sql}'";
                //LogHelper.Error<T>(sqlTxt + $" userId:{userId}");
                throw new Exception(sqlTxt);
            }
        }

        #endregion

        #region ListOrSafeListOrGet

        /// <summary>
        /// List ALL the T's (including the flagged as removed)
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> List(int userId, string tableName, string connectionString = null) => QuerySql($"SELECT * FROM {tableName};", userId, connectionString);
        public virtual IEnumerable<T> List(int userId, string connectionString = null) => QuerySql($"SELECT * FROM {_tableName};", userId, connectionString);

        /// <summary>
        /// List ALL the Y's (including the flagged as removed)
        /// Used to allow user to ask for a given type of data from a dal that manage another type of data
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Y> List<Y>(int userId, string tableName = null, string connectionString = null) where Y : class => QuerySql<Y>($"SELECT * FROM {tableName ?? _tableName};", userId, connectionString);

        /// <summary>
        /// List the T's that were not flagged as removed
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> SafeList(int userId, string tableName = null)
        {
            var sql = $"SELECT * FROM {tableName ?? typeof(T).Name} WHERE DeletionDate IS NULL;";
            return QuerySql(sql, userId);
        }

        /// <summary>
        /// Get a T object
        /// </summary>
        /// <param name="id">object id</param>
        /// <param name="userId"></param>
        /// <returns>T instance</returns>
        public virtual T Get(string id, int userId, string tableName = null)
        {
            var sql = $@"SELECT * FROM {tableName ?? _tableName} WHERE {ReflectionHelper.GetKeyPropertiesValues(typeof(T)).Single()} = '{id}' AND DeletionDate IS NULL";
            var item = QuerySql(sql, userId).SingleOrDefault();
            return item;
        }

        /// <summary>
        /// Get a T object
        /// </summary>
        /// <param name="id">object id</param>
        /// <param name="userId"></param>
        /// <returns>T instance</returns>
        public virtual T Get(int id, int userId, string tableName = null)
        {
            var sql = $@"SELECT * FROM {tableName ?? _tableName} WHERE {ReflectionHelper.GetKeyPropertiesValues(typeof(T)).Single()} = {id} AND DeletionDate IS NULL";
            var item = QuerySql(sql, userId).SingleOrDefault();
            return item;
        }

        #endregion

        #region AddingUpdatingDeleting

        /// <summary>
        /// Add or update an entity
        /// </summary>
        /// <param name="entity">entity to upsert</param>
        public virtual object AddOrUpdate(T entity, int userId, string __tableName = null)
        {
            var all = ReflectionHelper.GetPersistancePropertiesValues(entity);
            //var key = all.Where(a => a.Key.ToLower() == "Id".ToLower());
            var key = ReflectionHelper.GetKeyPropertiesValues(entity);
            if (!key.Any())
                throw new Exception("No PK defined");
            var values = all.Where(a => a.Key.ToLower() != "Id".ToLower());
            var tableName = !string.IsNullOrEmpty(_tableName) ? _tableName : entity.GetType().Name;

            string sql = null;

            if (Configuration.GetValue<string>("Data:DefaultConnection:ConnectionType") == "MySql")
            {
                sql = $@"INSERT INTO `{__tableName ?? tableName}` ( {string.Join(" , ", all.Select(a => $"`{a.Key}`"))} ) 
                    VALUES ( {string.Join(" , ", all.Select(a => $"{formatter(a.Value)}"))} )
                    ON DUPLICATE KEY UPDATE {string.Join(" , ", values.Select(a => $"{a.Key} = {formatter(a.Value)}"))};
                    SELECT LAST_INSERT_ID() as id;";
            }
            else
                sql =
                    $@"MERGE INTO {__tableName ?? tableName}
                   USING (SELECT {string.Join(", ", all.Select(a => $"{formatter(a.Value)} AS {a.Key}"))}) AS SRC ON {string.Join(" AND ", key.Select(a => $"{__tableName ?? tableName}.{a.Key} LIKE SRC.{a.Key}"))}
                   WHEN MATCHED THEN UPDATE SET {string.Join(" , ", values.Select(a => $"{a.Key} = {formatter(a.Value)}"))}
                   WHEN NOT MATCHED THEN INSERT({string.Join(",", values.Select(a => $"{a.Key}"))}) VALUES({string.Join(",", values.Select(a => $"@{a.Key}"))});
                   SELECT CAST(SCOPE_IDENTITY() as int); "; //Scope identity returns the index only in the case of an insert

            using (var conn = _getConnection(ConnectionString))
            {
                conn.Open();

                if (Configuration.GetValue<string>("Data:DefaultConnection:ConnectionType") == "MySql")
                {
                    var exec = conn.Query(sql, values).SingleOrDefault();
                    return (int)exec.id;
                }
                else
                {
                    var exec2 = conn.Query<int?>(sql, values).Single();
                    return (int)exec2;
                }
            }
        }

        public virtual void AddOrUpdate(IEnumerable<T> entities, int userId)
        {
            if (!entities.Any())
                return;

            foreach (var cur in entities)
            {
                AddOrUpdate(cur, userId);
            }
        }

        public virtual void SafeRemove(T entity, int userId)
        {
            var tableName = !string.IsNullOrEmpty(_tableName) ? _tableName : entity.GetType().Name;

            var keyAsDictionary = ReflectionHelper.GetKeyPropertiesValues(entity);
            if (!keyAsDictionary.Any())
                throw new Exception($"No Key has been defined for entity type {typeof(T)}");

            var sql = string.Empty;

            try
            {
                using (var conn = _getConnection(ConnectionString))
                {
                    conn.Open();
                    sql = $"SELECT COUNT(*) FROM {tableName} WHERE " + string.Join(" AND ", keyAsDictionary.Select(a => a.Key + "='" + a.Value + "'"));
                    var count = conn.Query<int>(sql).Single();
                    if (count != 1)
                        return;
                }
            }
            catch (SqlException)
            {
                var sqlTxt = $"SQL Query error, statement:'{sql}'";
                //LogHelper.Error<T>(sqlTxt + $" userId:{userId}");
                throw new Exception(sqlTxt);
            }
            try
            {
                sql = $"UPDATE {tableName} SET DeletionDate=now() WHERE " + string.Join(" AND ", keyAsDictionary.Select(a => a.Key + "='" + a.Value + "'"));
                //LogHelper.Info<T>(sql + $" userId:{userId}");
                using (var conn = _getConnection(ConnectionString))
                {
                    conn.Open();
                    conn.Execute(sql);
                }

            }
            catch (SqlException)
            {
                var sqlTxt = $"SQL Query error, statement:'{sql}'";
                //LogHelper.Error<T>(sqlTxt + $" userId:{userId}");
                throw new Exception(sqlTxt);
            }

        }
        #endregion
    }
}
