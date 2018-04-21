using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Models.Financial
{
    [DataContract]
    public class CustomerHasFavoritePortfolio
    {
        // [Key]
        // [DataMember]
        // public int id {get; set;}

        // [DataMember]
        // public string companyName { get; set; }
        public int customerId { get; set; }
        public string portfolioName { get; set; }
    }
}