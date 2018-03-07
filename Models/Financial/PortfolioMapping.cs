using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models.Abstract;

namespace YsrisCoreLibrary.Models.Financial
{
    /// <summary>
    /// 
    /// </summary>
    public class PortfolioMapping : AbstractEntity, IAbstractEntity
    {
        [Key]
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string portfolioCode { get; set; }

        [DataMember]
        public decimal? portfolioSize { get; set; }

        [DataMember]
        public bool isVisible { get; set; }

        [DataMember]
        public string portfolioCategoryCode { get; set; }

        [DataMember]
        [NotMapped]
        public bool existsLocally { get; set; }

        [DataMember]
        [NotMapped]
        public bool existtsRemotely { get; set; }
    }
}
