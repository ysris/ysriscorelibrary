using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models
{
    public interface ICustomerCompany
    {
        int? creatorCustomerId { get; }
        ICustomer creatorustomer { get; set; }
        string pictureClientAccessor { get; set; }
    }
}
