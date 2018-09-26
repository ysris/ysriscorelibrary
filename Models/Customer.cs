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
    public class Customer : IAbstractEntity, ICustomer
    {
        public Customer()
        {
        }

        public Customer(Customer values)
        {
            SetFromValues((IAbstractEntity)values);
        }

        public virtual void SetFromValues(ICustomer values)
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
            if (((ICustomer)values).adrLine2 != null) adrLine2 = ((ICustomer)values).adrLine2;
            if (((ICustomer)values).adrPostalCode != null) adrPostalCode = ((ICustomer)values).adrPostalCode;
            if (((ICustomer)values).adrCity != null) adrCity = ((ICustomer)values).adrCity;
            if (((ICustomer)values).adrCountry != null) adrCountry = ((ICustomer)values).adrCountry;
            isMailingSuscribed = ((ICustomer)values).isMailingSuscribed;
            if (((ICustomer)values).companyName != null) companyName = ((ICustomer)values).companyName;
            if (((ICustomer)values).phoneNumber != null) phoneNumber = ((ICustomer)values).phoneNumber;
            if (((ICustomer)values).freetext != null) freetext = ((ICustomer)values).freetext;
        }

        /// <summary>
        /// Primary key, AUTO_INCREMENT
        /// </summary>
        [Key]
        public int id { get; set; }

        public string picture { get; set; }

        /// <summary>
        /// Set by server with the api download pic endpoint
        /// </summary>
        [NotMapped]
        public string pictureClientAccessor { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public DateTime? deletionDate { get; set; }

        public DateTime? createdAt { get; set; }

        public string rolesString { get; set; }

        public string email { get; set; }

        [JsonIgnore]
        public string password { get; set; }

        public string passwordHash { get; set; }

        public string activationCode { get; set; }

        public string accountStatus { get; set; }

        public DateTime? recoverAskDate { get; set; }

        public string adrLine1 { get; set; }

        public string adrLine2 { get; set; }

        public string adrPostalCode { get; set; }

        public string adrCity { get; set; }

        public string adrCountry { get; set; }

        public bool isMailingSuscribed { get; set; }

        public string companyName { get; set; }

        public string phoneNumber { get; set; }

        public string freetext { get; set; }

        public int? companyId { get; set; }

        [NotMapped]
        public string rawPasswordConfirm { get; set; }

        [NotMapped]
        public string passwordForTyping { get; set; }

        [NotMapped]
        public virtual string prettyName => !(string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName)) ? $"{firstName} {lastName}" : email;

        [NotMapped]
        public List<string> roles => !string.IsNullOrEmpty(rolesString) ? rolesString.Split(',').Select(a => a.Trim()).ToList() : null;

        [NotMapped]
        public virtual string initials =>
            !(string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName))
                ? $"{(firstName ?? string.Empty).FirstOrDefault()} {(lastName ?? string.Empty).FirstOrDefault()}"
                : email;

    }
}
