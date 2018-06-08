using System.Collections.Generic;
using YsrisCoreLibrary.Models;

namespace ysriscorelibrary.Interfaces
{
    public interface IAbstractEntity
    {
        int id { get; set; }
        

        void SetFromValues(IAbstractEntity values);
    }
}