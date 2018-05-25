using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class CustomerCompany : IAbstractEntity
    {
        /// <summary>
        /// Primary key, AUTO_INCREMENT
        /// </summary>
        [DataMember]
        [Key]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string phoneNumber { get; set; }

        [DataMember]
        public string corporateEmail { get; set; }

        [DataMember]
        public string vat { get; set; }

        [DataMember]
        public DateTime creationDate { get; set; }
        [DataMember]
        public DateTime deletionDate { get; set; }

        [DataMember]
        public int creatorCustomerId { get; set; }

        [DataMember]
        [NotMapped]
        public Customer creatorustomer { get; set; }

        [DataMember]
        public string adrLine1 { get; set; }
        [DataMember]
        public string adrLine2 { get; set; }
        [DataMember]
        public string adrPostalCode { get; set; }
        [DataMember]
        public string adrCity { get; set; }
        [DataMember]
        public string adrCountry { get; set; }

        [DataMember]
        public string picture { get; set; }

        [NotMapped]
        [DataMember]
        public string prettyName => name;
    }
}
