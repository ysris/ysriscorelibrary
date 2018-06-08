using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ysriscorelibrary.Interfaces;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class Customer : IAbstractEntity, ICustomer
    {
        #region Constructors and initializers
        public Customer()
        {
        }

        public Customer(Customer values)
        {
            SetFromValues((IAbstractEntity)values);
        }

        public void SetFromValues(ICustomer values)
        {
            SetFromValues((IAbstractEntity)values);
        }

        public virtual void SetFromValues(IAbstractEntity values)
        {
            
            //if (values.id != null) id = values.id;
            if (((ICustomer)values).picture != null) picture = ((ICustomer)values).picture;
            if (((ICustomer)values).firstName != null) firstName = ((ICustomer)values).firstName;
            if (((ICustomer)values).lastName != null) lastName = ((ICustomer)values).lastName;
            if (((ICustomer)values).deletionDate != null) deletionDate = ((ICustomer)values).deletionDate;
            if (((ICustomer)values).email != null) email = ((ICustomer)values).email;
            if (((ICustomer)values).password != null) password = ((ICustomer)values).password;
            if (((ICustomer)values).activationCode != null) activationCode = ((ICustomer)values).activationCode;
            if (((ICustomer)values).adrLine1 != null) adrLine1 = ((ICustomer)values).adrLine1;
            if (((ICustomer)values).adrPostalCode != null) adrPostalCode = ((ICustomer)values).adrPostalCode;
            if (((ICustomer)values).adrCity != null) adrCity = ((ICustomer)values).adrCity;
            if (((ICustomer)values).adrCountry != null) adrCountry = ((ICustomer)values).adrCountry;
            if (((ICustomer)values).isMailingSuscribed != null) isMailingSuscribed = (bool)((ICustomer)values).isMailingSuscribed;
            if (((ICustomer)values).companyName != null) companyName = ((ICustomer)values).companyName;
            if (((ICustomer)values).phoneNumber != null) phoneNumber = ((ICustomer)values).phoneNumber;
            if (((ICustomer)values).freetext != null) freetext = ((ICustomer)values).freetext;
        }

        
        #endregion

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

        [DataMember]
        public string activationCode { get; set; }

        //[DataMember]
        //public string customerType { get; set; }

        [DataMember]
        public string accountStatus { get; set; }

        [DataMember]
        public DateTime? recoverAskDate { get; set; }


        [DataMember]
        public string adrLine1 { get; set; }
        [DataMember]
        public string adrPostalCode { get; set; }
        [DataMember]
        public string adrCity { get; set; }
        [DataMember]
        public string adrCountry { get; set; }

        [DataMember]
        public bool isMailingSuscribed { get; set; }

        [DataMember]
        public string companyName { get; set; }

        [DataMember]
        public string phoneNumber { get; set; }

        [DataMember]
        public string freetext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? companyId { get; set; }


        [DataMember]
        [NotMapped]
        public string rawPasswordConfirm { get; set; }

        [DataMember]
        [NotMapped]
        public string passwordForTyping { get; set; }


        [DataMember]
        [NotMapped]
        public virtual string prettyName => !(string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName)) ? $"{firstName} {lastName}" : email;

        [DataMember]
        [NotMapped]
        public List<string> roles => !string.IsNullOrEmpty(rolesString) ? rolesString.Split(',').Select(a => a.Trim()).ToList() : null;

        [DataMember]
        [NotMapped]
        public virtual string initials =>
            !(string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName))
                ? $"{(firstName ?? string.Empty).FirstOrDefault()} {(lastName ?? string.Empty).FirstOrDefault()}"
                : email;
        
    }
}
