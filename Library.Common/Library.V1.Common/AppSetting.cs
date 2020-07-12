using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Common
{
    public enum EnvirMode
    {
        Product = 0,
        Test = 1
    }
    public class AppSetting
    {
        public AppSetting()
        {
            this.Language = new LanguageSetting();
            this.Environment = new Dictionary<string, AppEnvir>();
            this.CurrentEnvir = EnvirMode.Product;
        }
        #region Public Fields
        public LanguageSetting Language { get; set; }
        public Dictionary<string, AppEnvir> Environment { get; set; }
        public EnvirMode CurrentEnvir { get; set; }
        // Create new Install each time 
        public SQLAccount DataAccount
        {
            get
            {
                if (this.Environment.ContainsKey(this.CurrentEnvir.ToString()))
                {
                    AppEnvir appEnvir = this.Environment[this.CurrentEnvir.ToString()];
                    appEnvir.Data.Lang = this.Lang;
                    return appEnvir.Data;
                }
                return new SQLAccount();
            }
        }
        public SQLAccount FileAccount
        {
            get
            {
                if (this.Environment.ContainsKey(this.CurrentEnvir.ToString()))
                {
                    AppEnvir appEnvir = this.Environment[this.CurrentEnvir.ToString()];
                    appEnvir.File.Lang = this.Lang;
                    return appEnvir.File;
                }
                return new SQLAccount();
            }
        }
        public string Lang
        {
            get
            {
                // if current empty, current = "" 
                // if default empty, default = "en"
                // if current not in accept list, current = default
                // if current = "zh",  current = "cn"
                // if current still empty,  current = default 
                if (string.IsNullOrWhiteSpace(this.Language.Current)) this.Language.Current = "";
                if (string.IsNullOrWhiteSpace(this.Language.Default)) this.Language.Default = "en";
                if (!this.Language.Accept.Contains(this.Language.Current.ToLower())) this.Language.Current = this.Language.Default;
                if (this.Language.Current.ToLower() == "zh") this.Language.Current = "cn";
                if (string.IsNullOrWhiteSpace(this.Language.Current))
                {
                    this.Language.Current = this.Language.Default;
                }
                return this.Language.Current.ToLower();
            }
            set
            {
                this.Language.Current = value;
            }
        }
        #endregion
    }
    public class AppEnvir
    {
        public AppEnvir()
        {
            this.Data = new SQLAccount();
            this.File = new SQLAccount();
        }
        #region Public Fields
        public SQLAccount Data { get; set; }
        public SQLAccount File { get; set; }
        #endregion
    }
    public class SQLAccount
    {
        public SQLAccount()
        {
        }
        #region Public Fields
        public string Lang { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool Trust { get; set; }
        public bool IsDebug { get; set; }
        public string ConnectString
        {
            get
            {
                if (Trust)
                    return $"Data Source={this.Server};Initial Catalog={this.Database};Integrated Security=True;Connection Timeout=60;";
                else
                    return $"Data Source={this.Server};Initial Catalog={this.Database};Persist Security Info=True;User ID={this.User};Password={this.Password};Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=60;";
            }
        }
        #endregion
    }
    public class LanguageSetting
    {
        public LanguageSetting()
        {
            this.Accept = new List<string>();
        }
        #region Public Fields
        public string Current { get; set; }
        public string Default { get; set; }
        public List<string> Accept { get; set; }
        #endregion
    }
}
