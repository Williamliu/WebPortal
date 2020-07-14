using Library.V1.SQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Entity
{
    public enum ERef
    {
        None = 0,
        O2O = 1,
        O2M = 2
    }
    public class Relation
    {
        public Relation()
        {
            this.RefKey = 0;
            this.RefType = ERef.None;
            this.ForeignKey = string.Empty;
        }
        public Relation(ERef rtype, string rfkey, int rkey = 0)
        {
            this.RefType = rtype;
            this.ForeignKey = rfkey;
            this.RefKey = rkey;
        }
        public int RefKey { get; set; }
        public ERef RefType { get; set; }
        public string ForeignKey { get; set; }

        public SQLRow SQLRowInsert
        {
            get
            {
                SQLRow row = new SQLRow();
                switch (this.RefType)
                {
                    case ERef.None:
                        break;
                    case ERef.O2O:
                    case ERef.O2M:
                        row.Add(this.ForeignKey, this.ForeignKey, this.RefKey);
                        break;
                }
                return row;
            }
        }
        public SQLWhereSearch WhereSearchGet
        {
            get
            {
                SQLWhereSearch search = new SQLWhereSearch();
                switch (this.RefType)
                {
                    case ERef.None:
                        break;
                    case ERef.O2O:
                    case ERef.O2M:
                        search.Add(this.ForeignKey, this.RefKey);
                        break;
                }
                return search;
            }
        }
        public SQLWhereSearch WhereSearchUpdate
        {
            get
            {
                SQLWhereSearch search = new SQLWhereSearch();
                switch (this.RefType)
                {
                    case ERef.None:
                        break;
                    case ERef.O2O:
                    case ERef.O2M:
                        search.Add(this.ForeignKey, this.RefKey);
                        break;
                }
                return search;
            }
        }
        public SQLWhereSearch WhereSearchDelete
        {
            get
            {
                SQLWhereSearch search = new SQLWhereSearch();
                switch (this.RefType)
                {
                    case ERef.None:
                        break;
                    case ERef.O2O:
                    case ERef.O2M:
                        search.Add(this.ForeignKey, this.RefKey);
                        break;
                }
                return search;
            }
        }
    }
}
