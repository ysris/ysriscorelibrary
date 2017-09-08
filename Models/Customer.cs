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
using YsrisSaas2.Models;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class Customer : AbstractEntity
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
            if (values.customerMainAdress != null) customerMainAdress = new PostalAddress(values.customerMainAdress);
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

        ///// <summary>
        ///// 
        ///// </summary>
        //[DataMember]
        //[NotMapped]
        //public string passwordForTyping { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        [DataMember]
        public string activationCode { get; set; }

        [DataMember]
        public Role customerType { get; set; }

        [DataMember]
        public CustomerStatus accountStatus { get; set; }

        [DataMember]
        public DateTime? recoverAskDate { get; set; }


        [DataMember]
        public int? customerMainAdressId { get; set; }
        [DataMember]
        public PostalAddress customerMainAdress { get; set; }


        /// <summary>
        /// Contains the full list of controller/actions assignable to the user
        /// </summary>
        [DataMember]
        [CheckBoxList]
        [NotMapped]
        public List<CustomerHasModule> menuItemsFullList { get; set; }

        /// <summary>
        /// User for listbox callback
        /// </summary>
        [DataMember]
        [NotMapped]
        public List<string> checkBoxListSelectedValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [NotMapped]
        public List<CustomerHasModule> userRights { get; set; }

        [DataMember]
        [NotMapped]
        public virtual string prettyName => !(string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName)) ? $"{lastName} {firstName}" : email;

        [DataMember]
        [NotMapped]
        public List<Role> roles => !string.IsNullOrEmpty(rolesString) ? rolesString.Split(',').Select(b => (Role)Enum.Parse(typeof(Role), b)).ToList() : null;



        public override string ToString() => string.Join(", ", this.GetType().GetProperties().Select(a => string.Concat(a.Name, '=', this.GetType().GetProperty(a.Name).GetValue(this))));


        [DataMember]
        [NotMapped]
        public bool isCustomerProfileComplete =>
            firstName != null
            && lastName != null
            && customerMainAdress?.adrCity != null
            && customerMainAdress?.adrPostalCode != null
            && customerMainAdress?.adrLine1 != null
            && customerMainAdress?.adrCountry != null;



    }
}
