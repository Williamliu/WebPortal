using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Entity
{
    public class Column
    {
        public Column()
        {
            this.Name = "";
            this.Guid = Guid.NewGuid();
            this.State = EState.Normal;
            this.Value = null;
            this.Value1 = null;
        }
        public Column(string name, object value = null, object value1 = null) : this()
        {
            this.Name = name;
            this.Value = value;
            this.Value1 = value1;
        }
        #region Fields
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public EState State { get; set; }
        public object Value { get; set; }
        public object Value1 { get; set; }
        #endregion
    }
}
