using System;
using System.Collections.Generic;

namespace YsrisCoreLibrary.Models
{
    public interface ICustomer
    {
        string accountStatus { get; set; }
        string activationCode { get; set; }
        string adrCity { get; set; }
        string adrCountry { get; set; }
        string adrLine1 { get; set; }
        string adrPostalCode { get; set; }
        DateTime? createdAt { get; set; }
        //string customerType { get; set; }
        DateTime? deletionDate { get; set; }
        string email { get; set; }
        string firstName { get; set; }
        int id { get; set; }
        string lastName { get; set; }
        string password { get; set; }
        string passwordHash { get; set; }
        string picture { get; set; } //TO REMOVE
        string pictureClientAccessor { get; set; }
        string prettyName { get; }
        DateTime? recoverAskDate { get; set; }
        List<string> roles { get; }
        string rolesString { get; set; }
        bool isMailingSuscribed { get; set; }
        string companyName { get; set; }
        string phoneNumber { get; set; }
        string freetext { get; set; }
        string rawPasswordConfirm { get; set; }
        string passwordForTyping { get; set; }
        void SetFromValues(ICustomer values);
        int? companyId { get; set; }
    }
}