using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models
{
    public class CustomerHasModule
    {

        public int Id { get; set; }
        public int CustomerId { get; set; }

        public string areaName { get; set; }
        public string controllerName { get; set; }
        public string actionName { get; set; }
        public string httpMethod { get; set; }

        public string prettyName => $"{areaName}:{controllerName}:{actionName}:{httpMethod}";
        public string encodedName => $"{areaName}:{controllerName}:{actionName}:{httpMethod}";

        public DateTime CreationDate { get; set; }
        public DateTime DeletionDate { get; set; }

        public IEnumerable<Type> actionAttributes { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            var entity = obj as CustomerHasModule;
            if (entity == null)
                return false;

            return
                entity.areaName == areaName
                && entity.controllerName == controllerName
                && entity.actionName == actionName
                && entity.httpMethod == httpMethod
                //&& entity.CreationDate == CreationDate
                //&& entity.DeletionDate == DeletionDate
                ;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
