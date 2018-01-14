using System;
using System.Collections.Generic;
using System.Text;

namespace YsrisCoreLibrary.Models
{
    public class TableStateEntity<T> where T : class
    {
        public TableStateSortEntity sort { get; set; }  //"{\"sort\":{\"predicate\":\"curCol.name\",\"reverse\":false}
        public TableStateSearchEntity search { get; set; } //,\"search\":{\"predicateObject\":{\"id\":\"395\"}},
        public dynamic pagination { get; set; }//start\":0,\"totalItemCount\":0,\"number\":10

        public class TableStateSearchEntity
        {
            public T predicateObject;
        }
    }

    public class GlobalSearchTableStateEntity
    {
        public string any { get; set; }
        public string instrumentcategorykey { get; set; }
    }

    public class GlobalSearchPortfolioTableStateEntity
    {
        public string any { get; set; }
        public string portfoliocategorykey { get; set; }
    }

    public class TableStateSortEntity
    {
        public string predicate { get; set; }
        public bool reverse { get; set; }
    }

}
