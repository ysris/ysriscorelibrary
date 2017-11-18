using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Models.Abstract;

namespace YsrisCoreLibrary.Models.Financial
{
    [DataContract]
    public class CustomerHasFavoriteInstrument
    {
        // [Key]
        // [DataMember]
        // public int id {get; set;}

        // [DataMember]
        // public string companyName { get; set; }
        public int customerId { get; set; }
        public int instrumentId { get; set; }
    }
}