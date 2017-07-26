using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YsrisSaas2.Models
{    
    public enum Role
    {
        Administrator = 1,          //FULL RIGHTS ON EVERYTHING
        User = 3,
        Proprietaire,
        Coach,
        Locataire,
        Business
    }
}
