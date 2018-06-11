using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;

namespace YsrisCoreLibrary.Models.Todo
{
    [DataContract]
    public class AbstractTodo : IAbstractEntity, ITodo
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public int creatorCustomerId { get; set; }
        [DataMember]
        [NotMapped]
        public ICustomer creatorCustomer { get; set; }
        [DataMember]
        public DateTime creationDate { get; set; }
        [DataMember]
        public DateTime? deletionDate { get; set; }

        public void SetFromValues(IAbstractEntity values)
        {
            throw new NotImplementedException();
        }
    }

    //TODO : Check to put in separate table
    public class AbstractTodoHistory : ITodo
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public DateTime updateDate { get; set; }

        [DataMember]
        public int todoId { get; set; }

        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public int creatorCustomerId { get; set; }
        [DataMember]
        [NotMapped]
        public ICustomer creatorCustomer { get; set; }
        [DataMember]
        public DateTime creationDate { get; set; }
        [DataMember]
        public DateTime? deletionDate { get; set; }

        public void SetFromValues(IAbstractEntity values)
        {
            throw new NotImplementedException();
        }


    }

    public interface ITodo
    {
        DateTime creationDate { get; set; }
        ICustomer creatorCustomer { get; set; }
        int creatorCustomerId { get; set; }
        DateTime? deletionDate { get; set; }
        string description { get; set; }
        int id { get; set; }
        string status { get; set; }
        string title { get; set; }

        void SetFromValues(IAbstractEntity values);
    }
}
