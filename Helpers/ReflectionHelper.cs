using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Abstract;
using YsrisCoreLibrary.Attributes;
using YsrisCoreLibrary.Extensions;

namespace YsrisCoreLibrary.Helpers
{
    public class ReflectionHelper
    {
        public static Func<Type, PropertyInfo[]> GetPropertiesOfEntityType = t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        public static Func<object, PropertyInfo[]> GetPropertiesOfEntity = t => t.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        public static Dictionary<string, object> GetPersistancePropertiesValues(object entity)
        {
            if (entity == null)
                return null;

            var qry =
                from x in GetPropertiesOfEntityType(entity.GetType())
                where !x.GetCustomAttributes().Select(a => a.GetType()).Contains(typeof(NotMappedAttribute))
                where !x.GetCustomAttributes().Select(a => a.GetType()).Contains(typeof(NotOrmMappedAttribute))
                where
                    x.CanWrite
                //only with setter
                where
                        x.PropertyType == typeof(string)
                        || x.PropertyType == typeof(DateTime)
                        || x.PropertyType == typeof(DateTime?)
                        || x.PropertyType == typeof(int)
                        || x.PropertyType == typeof(int?)
                        || x.PropertyType == typeof(float)
                        || x.PropertyType == typeof(float?)
                        || x.PropertyType == typeof(decimal)
                        || x.PropertyType == typeof(decimal?)
                        || x.PropertyType == typeof(byte[])
                        || x.PropertyType.GetTypeInfo().BaseType == typeof(Enum)
                let prop = entity.GetType().GetProperty(x.Name)
                let pVal = prop.GetValue(entity, null)
                let val =
                    pVal != null && pVal.GetType().GetTypeInfo().BaseType == typeof(Enum)
                        ? pVal.ToString()
                        : pVal
                where (
                    (
                        x.PropertyType == typeof(DateTime)
                        && (DateTime)val != DateTime.MinValue
                    )
                    ||
                    x.PropertyType != typeof(DateTime)
                )
                select new { prop, val };

            return qry.ToDictionary(a => a.prop.Name, a => a.val);
        }

        public static IEnumerable<CustomerHasModule> GetAvailableMenuItems(Type oneTypeOfThessembly)
        {
            var controllersSet =
                from x in oneTypeOfThessembly.GetTypeInfo().Assembly.GetTypes()
                where (
                x.GetTypeInfo().BaseType.Equals(typeof(Controller))
                || x.GetTypeInfo().BaseType.Equals(typeof(Controller))
                )
                where !x.Equals(typeof(Controller))
                select x;

            var qry =

                from x in controllersSet.SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                where x.IsPublic
                let LHttpAttribute = x.GetCustomAttributes(typeof(HttpMethodAttribute)).Select(b => b.GetType().Name.ToString().TrimEnd("Attribute", StringComparison.CurrentCulture)).SingleOrDefault()
                select new CustomerHasModule
                {
                    controllerName = x.DeclaringType.Name,
                    areaName = x.DeclaringType.GetTypeInfo().GetCustomAttributes(typeof(AreaAttribute)).Select(b => ((AreaAttribute)b).RouteValue).SingleOrDefault(),
                    actionName = x.Name,
                    httpMethod = string.IsNullOrEmpty(LHttpAttribute) ? "HttpGet" : LHttpAttribute,
                    actionAttributes = x.GetCustomAttributes().Select(a => a.GetType())
                };

            return qry;
        }



        public static IEnumerable<string> GetKeyPropertiesValues(Type entity)
        {
            if (entity == null)
                return null;

            var qry =
                from x in GetPropertiesOfEntityType(entity)
                where x.GetCustomAttributes().Select(a => a.GetType()).Contains(typeof(KeyAttribute))
                select x.Name;

            return qry;
        }

        public static Dictionary<string, object> GetKeyPropertiesValues(object entity)
        {
            if (entity == null)
                return null;

            var qry =
                from x in GetPropertiesOfEntityType(entity.GetType())
                where x.GetCustomAttributes().Select(a => a.GetType()).Contains(typeof(KeyAttribute))
                let prop = entity.GetType().GetProperty(x.Name)
                let val = prop.GetValue(entity, null)
                select new { prop, val };

            return qry.ToDictionary(a => a.prop.Name, a => a.val);
        }


        public static IEnumerable<PurpleColumn> GetColumns(object entity)
        {
            if (entity == null)
                return null;

            var qry =
                from a in entity is Type ? GetPropertiesOfEntityType((Type)entity) : GetPropertiesOfEntity(entity)
                where a.CanWrite

                let LProperty =
                    Nullable.GetUnderlyingType(a.PropertyType) != null
                        ? Nullable.GetUnderlyingType(a.PropertyType)
                        : a.PropertyType

                let LIsKey = a.GetCustomAttributes().Select(b => b.GetType()).Contains(typeof(KeyAttribute))
                let LIsRequired = a.GetCustomAttributes().Select(b => b.GetType()).Contains(typeof(RequiredAttribute))
                let LIsBrowsableFlase = a.GetCustomAttributes().Select(b => b.GetType()).Contains(typeof(BrowsableFalseAttribute))

                let LIsCheckBoxList = a.GetCustomAttributes(typeof(CheckBoxListAttribute)).Any()

                let LIsEnum = LProperty.GetTypeInfo().IsEnum


                where !LIsBrowsableFlase

                let Ltype = LIsKey
                            ? "Key"
                            : LIsCheckBoxList
                                ? "CheckBoxList"
                            : LProperty.Name.StartsWith("List")
                                ? "List"
                                : LIsEnum
                                    ? "Enum"
                                    : LProperty.Name

                select new PurpleColumn
                {
                    name = a.Name,
                    type = Ltype,
                    isRequired = LIsRequired,
                    entityColumnsModel =
                    LIsEnum
                        ? (object)Enum.GetValues(LProperty)
                        : GetColumns(LProperty.GenericTypeArguments.FirstOrDefault()),


                };
            return qry;
        }


    }
}
