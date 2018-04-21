using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Models.Financial
{
    [DataContract]
    public class CustomerHasFavoriteInstrument
    {
        public int customerId { get; set; }
        public int instrumentId { get; set; }
    }
}