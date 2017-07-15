using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Models.Abstract
{
    [DataContract]
    public abstract class AbstractEntity
    {
        //[DataMember]
        //public Dictionary<string, PurpleColumn> entityColumnsModel { get; set; }

        public bool IsPropertyExist(dynamic obj, string name)
        {
            if (obj is ExpandoObject)
                return ((IDictionary<string, object>)obj).ContainsKey(name);

            return obj.GetType().GetProperty(name) != null;
        }
    }
}
