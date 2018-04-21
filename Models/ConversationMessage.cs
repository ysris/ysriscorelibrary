using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class ConversationMessage : IAbstractEntity
    {

        public ConversationMessage()
        {

        }

        public ConversationMessage(dynamic values)
        {
            SetFromValues(values);
        }

        public void SetFromValues(dynamic values)
        {
            if (values.id != null) id = values.id;
            if (values.destId != null) destId = values.destId;
            if (values.message != null) message = values.message;
        }


        /// <summary>
        /// Primary key, AUTO_INCREMENT
        /// </summary>
        [DataMember]
        [Key]
        public int id { get; set; }

        [DataMember]
        public int authorId { get; set; }

        [DataMember]
        public int destId { get; set; }

        [DataMember]
        public DateTime creationDate { get; set; }

        [DataMember]
        [NotMapped]
        public string creationDateFormatted => creationDate.ToString("dddd dd MMMM, hh:mm");

        [DataMember]
        public string message { get; set; }

        [DataMember]
        public Customer author { get; set; }

        [DataMember]
        public Customer dest { get; set; }

        [DataMember]
        public bool isDaySwitch { get; set; }

        [DataMember]
        public bool isReadByDest { get; set; }

        [DataMember]
        [NotMapped]
        public bool isConnectedUserAuthor { get; set; }
    }
}
