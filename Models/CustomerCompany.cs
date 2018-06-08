using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models.Stripe;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class CustomerCompany : IAbstractEntity, ICustomerCompany
    {
        public CustomerCompany()
        {

        }

        public CustomerCompany(dynamic values)
        {
            SetFromValues((IAbstractEntity)values);
        }

        public void SetFromValues(IAbstractEntity values)
        {
            var cast = ((CustomerCompany)values);


            if (cast.id != null) id = cast.id;
            if (cast.name != null) name = cast.name;
            if (cast.phoneNumber != null) phoneNumber = cast.phoneNumber;
            if (cast.corporateEmail != null) corporateEmail = cast.corporateEmail;
            if (cast.vat != null) vat = cast.vat;
            if (cast.creationDate != null) creationDate = cast.creationDate;
            if (cast.deletionDate != null) deletionDate = cast.deletionDate;
            if (cast.creatorCustomerId != null) creatorCustomerId = cast.creatorCustomerId;
            if (cast.adrLine1 != null) adrLine1 = cast.adrLine1;
            if (cast.adrPostalCode != null) adrPostalCode = cast.adrPostalCode;
            if (cast.adrCity != null) adrCity = cast.adrCity;
            if (cast.adrCountry != null) adrCountry = cast.adrCountry;
            if (cast.picture != null) picture = cast.picture;
            if (cast.pictureClientAccessor != null) pictureClientAccessor = cast.pictureClientAccessor;
            if (cast.subscribedcustomersCount != null) subscribedcustomersCount = cast.subscribedcustomersCount;
            if (cast.billedAmount != null) billedAmount = cast.billedAmount;
            if (cast.availableLicenceCount != null) availableLicenceCount = cast.availableLicenceCount;
            if (cast.totalLicenceCount != null) totalLicenceCount = cast.totalLicenceCount;
        }


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
        public DateTime? creationDate { get; set; }
        [DataMember]
        public DateTime? deletionDate { get; set; }

        [DataMember]
        public int? creatorCustomerId { get; set; }

        [DataMember]
        [NotMapped]
        public ICustomer creatorustomer { get; set; }

        [DataMember]
        public string adrLine1 { get; set; }
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

        [DataMember]
        [NotMapped]
        public string pictureClientAccessor { get; set; }

        [DataMember]
        [NotMapped]
        public CustomerCompanyStripeSubscription subscription { get; set; }

        [DataMember]
        [NotMapped]
        public List<Customer> subscribedcustomers { get; set; }

        [DataMember]
        [NotMapped]
        public int subscribedcustomersCount { get; set; }

        [DataMember]
        [NotMapped]
        public object billedAmount { get;  set; }

        [DataMember]
        [NotMapped]
        public int availableLicenceCount { get; set; }

        [DataMember]
        [NotMapped]
        public int totalLicenceCount { get; set; }
    }
}
