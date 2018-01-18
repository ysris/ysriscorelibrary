using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YsrisCoreLibrary.Attributes;
using YsrisCoreLibrary.Enums;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Models.Abstract;
using ysriscorelibrary.Interfaces;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class Customer : AbstractEntity, IAbstractEntity
    {
        public Customer()
        {
        }

        public Customer(dynamic values)
        {
            SetFromValues(values);
        }

        public void SetFromValues(dynamic values)
        {
            if (values.id != null) id = values.id;
            if (values.picture != null) picture = values.picture;
            if (values.firstName != null) firstName = values.firstName;
            if (values.lastName != null) lastName = values.lastName;
            if (values.deletionDate != null) deletionDate = values.deletionDate;
            if (values.email != null) email = values.email;
            if (values.password != null) password = values.password;
            if (values.activationCode != null) activationCode = values.activationCode;
            if (values.adrLine1 != null) adrLine1 = values.adrLine1;
            if (values.adrLine2 != null) adrLine2 = values.adrLine2;
            if (values.adrPostalCode != null) adrPostalCode = values.adrPostalCode;
            if (values.adrCity != null) adrCity = values.adrCity;
            if (values.adrCountry != null) adrCountry = values.adrCountry;
        }

        /// <summary>
        /// Primary key, AUTO_INCREMENT
        /// </summary>
        [DataMember]
        [Key]
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string picture { get; set; }

        /// <summary>
        /// Set by server with the api download pic endpoint
        /// </summary>
        [DataMember]
        [NotMapped]
        public string pictureClientAccessor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? deletionDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? createdAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string rolesString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [JsonIgnore]
        public string password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        [DataMember]
        public string activationCode { get; set; }

        [DataMember]
        public string customerType { get; set; }

        [DataMember]
        public string accountStatus { get; set; }

        [DataMember]
        public DateTime? recoverAskDate { get; set; }

        
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
        public string apiKey { get; set; }


        [DataMember]
        [NotMapped]
        public virtual List<object> entityModel { get; set; } =
            new List<object>
            {
                // new { name = "clientId",type="select"},
            };

        [DataMember]
        [NotMapped]
        public virtual string prettyName => !(string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName)) ? $"{firstName} {lastName}" : email;

        [DataMember]
        [NotMapped]
        public List<string> roles => !string.IsNullOrEmpty(rolesString) ? rolesString.Split(',').ToList() : null;
    }
}
