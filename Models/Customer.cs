﻿using System;
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
            SetFromValues(values);
        }

        public virtual void SetFromValues(ICustomer values)
        {
            //if (values.id != null) id = values.id;
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
            if (values.isMailingSuscribed != null) isMailingSuscribed = (bool)values.isMailingSuscribed;
            if (values.companyName != null) companyName = values.companyName;
            if (values.phoneNumber != null) phoneNumber = values.phoneNumber;
            if (values.freetext != null) freetext = values.freetext;
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
        public string adrLine2 { get; set; }
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
