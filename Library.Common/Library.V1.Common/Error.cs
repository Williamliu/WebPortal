using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Common
{
    public enum ErrorCode
    {
        None = 0,
        Other = 10,
        Gallery = 100,
        Image = 101,
        ImageKey = 102,
        SQLConnection = 1000,
        SQLQuery = 1100,
        SQLNoMeta = 1200,
        FilterValidate = 2000,
        ValueValidate = 2100,
        UserRight = 4001,
        LoginFail = 8001,
        NotFound = 8011,
        Authorization = 9999
    }
    public class Error
    {
        public Error()
        {
            this.Code = ErrorCode.None;
            this.Memo = string.Empty;
            this.Messages = new List<string>();
        }
        public Error(ErrorCode code, string message, string memo = "") : this()
        {
            this.Code = code;
            this.Messages.Add(message);
            this.Memo = memo;
        }
        public Error(Error error) : this()
        {
            this.Code = error.Code;
            this.Messages = error.Messages;
            this.Memo = error.Memo;
        }

        #region fields
        public ErrorCode Code { get; set; }
        public string Memo { get; set; }
        public List<string> Messages { get; set; }
        #endregion

        #region Property
        [JsonIgnore]
        public bool HasError
        {
            get
            {
                return this.Code > ErrorCode.None ? true : false;
            }
        }
        [JsonIgnore]
        public int Count
        {
            get
            {
                return this.Messages.Count;
            }
        }
        #endregion

        #region Methods
        public void Append(ErrorCode code, string message, string memo = "")
        {
            if (code == ErrorCode.None) return;
            if (code > this.Code)
            {
                this.Code = code;
                if (!string.IsNullOrWhiteSpace(memo)) this.Memo = memo;
            }
            //if error occur, always add message to list
            this.Messages.Add(message);
            if (string.IsNullOrWhiteSpace(this.Memo)) this.Memo = memo;
        }
        private void Append(ErrorCode code, List<string> messages, string memo = "")
        {
            if (code == ErrorCode.None) return;
            if (code > this.Code)
            {
                this.Code = code;
                if (!string.IsNullOrWhiteSpace(memo)) this.Memo = memo;
            }
            //if error occur, always add message to list
            foreach (string msg in messages) this.Messages.Add(msg);
            if (string.IsNullOrWhiteSpace(this.Memo)) this.Memo = memo;
        }
        public void Append(Error error)
        {
            this.Append(error.Code, error.Messages, error.Memo);
        }
        public void Reset()
        {
            this.Code = ErrorCode.None;
            this.Messages.Clear();
            this.Memo = string.Empty;
        }
        #endregion
    }
}
