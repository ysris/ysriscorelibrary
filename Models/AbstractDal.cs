using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using YsrisCoreLibrary.Extensions;
using YsrisCoreLibrary.Helpers;

namespace YsrisCoreLibrary.Dal
{
    public abstract class AbstractDal<T> where T : class
    {
        public AbstractDal()
        {
            _tableName = typeof(T).Name;
            ConnectionString = ConfigurationHelper.ConnectionString;
        }

        protected virtual string _tableName { get; }
        protected virtual string ConnectionString { get; }

        Func<object, string> formatter = a =>
            a == null ? "null"
            : a is string ? $"'{a.ToString()}'"
            : (a is DateTime || a is DateTime?) ? $"'{((DateTime)a).ToString("yyyy-MM-dd HH:mm:ss")}'"
            : a is int || a is decimal || a is decimal? || a is int? || a is double || a is float ? "'" + a.ToString().Replace(",", ".") + "'"
            : a.GetType().GetTypeInfo().BaseType == typeof(Enum) ? $"'{a.ToString()}'"
            : a.ToString();

        public IEnumerable<T> QuerySql(string sqlStatement, int userId, string connectionString = null) => QuerySql<T>(sqlStatement, userId, connectionString);
        //public IEnumerable<T> QuerySql(string sqlStatement, params object[] args) => QuerySql(string.Format(sqlStatement, args));
        public IEnumerable<Y> QuerySql<Y>(string sql, int userId, string connectionString = null) where Y : class
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString ?? ConnectionString))
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
                using (SqlConnection conn = new SqlConnection(connectionString ?? ConnectionString))
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

        /// <summary>
        /// Add or update an entity
        /// </summary>
        /// <param name="entity">entity to upsert</param>
        public virtual object AddOrUpdate(T entity, int userId)
        {
            var all = ReflectionHelper.GetPersistancePropertiesValues(entity);
            //var key = all.Where(a => a.Key.ToLower() == "Id".ToLower());
            var key = ReflectionHelper.GetKeyPropertiesValues(entity);
            var values = all.Where(a => a.Key.ToLower() != "Id".ToLower());
            var tableName = !string.IsNullOrEmpty(_tableName) ? _tableName : entity.GetType().Name;

            var sql =
                $@"MERGE INTO [{tableName}]
                   USING (SELECT {string.Join(", ", all.Select(a => $"{formatter(a.Value)} AS [{a.Key}]"))}) AS SRC ON {string.Join(" AND ", key.Select(a => $"[{tableName}].[{a.Key}] LIKE SRC.[{a.Key}]"))}
                   WHEN MATCHED THEN UPDATE SET {string.Join(" , ", values.Select(a => $"[{a.Key}] = {formatter(a.Value)}"))}
                   WHEN NOT MATCHED THEN INSERT({string.Join(",", values.Select(a => $"[{a.Key}]"))}) VALUES({string.Join(",", values.Select(a => $"@{a.Key}"))});
                   SELECT CAST(SCOPE_IDENTITY() as int); "; //Scope identity returns the index only in the case of an insert


            //LogHelper.Debug<T>($"AddOrUpdate of UserId:{userId}");
            //LogHelper.Debug<T>(
            //    sql
            //    //.Replace(Environment.NewLine, " ")
            //    .Replace("                   ", "")
            //);
            //LogHelper.Debug<T>(string.Join(",", all.Select(a => a.Key + ":" + a.Value)));

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var exec = conn.Query<int?>(sql, values).Single();

                if (exec != null)
                    return (int)exec;

                if (key.Count() == 1)
                {
                    if (key.Single().Value is int)
                        return (int)key.Single().Value;
                    if (key.Single().Value is int)
                        return key.Single().Value.ToString();
                }
                return 0;
            }
        }
        //public virtual void AddOrUpdate(IEnumerable<T> entities, int userId)
        //{
        //    if (!entities.Any())
        //        return;

        //    var partititioned = entities.Partitionate<T>(partitionSize: 500);


        //    using (SqlConnection conn = new SqlConnection(ConnectionString))
        //    {
        //        conn.Open();
        //        foreach (var curSet in partititioned)
        //        {
        //            //using (var t = Connection.BeginTransaction())
        //            //{
        //            foreach (var curEntity in curSet)
        //                AddOrUpdate(curEntity, userId);
        //            //t.Commit();
        //            //}
        //        }
        //    }
        //}

        /// <summary>
        /// List the T's that were not flagged as removed
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> SafeList(int userId) => QuerySql($"SELECT * FROM [{typeof(T).Name}] WHERE DeletionDate IS NULL;", userId);
        /// <summary>
        /// List ALL the T's (including the flagged as removed)
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> List(int userId, string connectionString=null) => QuerySql($"SELECT * FROM {_tableName};", userId, connectionString);

        public virtual T Get(string id, int userId)
        {
            var sql = $@"SELECT * FROM {_tableName} WHERE {ReflectionHelper.GetKeyPropertiesValues(typeof(T)).Single()} = '{id}' AND DeletionDate IS NULL";
            var item = QuerySql(sql, userId).SingleOrDefault();
            return item;
        }

        public virtual T Get(int id, int userId)
        {
            var sql = $@"SELECT * FROM {_tableName} WHERE {ReflectionHelper.GetKeyPropertiesValues(typeof(T)).Single()} = {id} AND DeletionDate IS NULL";
            var item = QuerySql(sql, userId).SingleOrDefault();
            return item;
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
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
                sql = $"UPDATE {tableName} SET DeletionDate=getdate() WHERE " + string.Join(" AND ", keyAsDictionary.Select(a => a.Key + "='" + a.Value + "'"));
                //LogHelper.Info<T>(sql + $" userId:{userId}");
                using (SqlConnection conn = new SqlConnection(ConnectionString))
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
        //public virtual void Remove(T entity, int userId)
        //{
        //    var tableName = !string.IsNullOrEmpty(_tableName) ? _tableName : entity.GetType().Name;

        //    var keyAsDictionary = ReflectionHelper.GetKeyPropertiesValues(entity);
        //    if (!keyAsDictionary.Any())
        //        throw new Exception($"No Key has been defined for entity type {typeof(T)}");

        //    string sql = string.Empty;

        //    try
        //    {
        //        sql = $"SELECT COUNT(*) FROM {tableName} WHERE " + string.Join(" AND ", keyAsDictionary.Select(a => a.Key + "='" + a.Value + "'"));
        //        var count = Connection.Query<int>(sql).Single();

        //        if (count != 1)
        //            return;
        //    }
        //    catch (SqlException)
        //    {
        //        var sqlTxt = $"SQL Query error, statement:'{sql}'";
        //        LogHelper.Error<T>(sqlTxt + $" userId:{userId}");
        //        throw new Exception(sqlTxt);
        //    }

        //    sql = "DELETE FROM " + tableName + " WHERE " + string.Join(" AND ", keyAsDictionary.Select(a => a.Key + "='" + a.Value + "'"));
        //    QuerySql(sql);
        //}




    }
}
