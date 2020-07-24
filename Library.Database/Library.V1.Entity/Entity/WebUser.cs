using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.V1.Entity
{
    public class WebUser
    {
        public WebUser()
        {
            this.Branches = new List<int>();
            this.ActiveBranches = new List<int>();
            this.Sites = new List<int>();
            this.ActiveSites = new List<int>();
            this.Fields = new List<string>();
            this.PrivateMenuIDs = new List<string>();
            this.PublicMenuIDs = new List<string>();
            this.Rights = new Dictionary<string, bool>();
            this.Items = new Dictionary<string, object>();
        }
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string FirstName { get; set; }
        [JsonIgnore]
        public string LastName { get; set; }
        [JsonIgnore]
        public string UserName { get; set; }
        [JsonIgnore]
        public string Email { get; set; }
        [JsonIgnore]
        public string Phone { get; set; }
        [JsonIgnore]
        public string ImageUrl { get; set; }
        [JsonIgnore]
        public int Branch { get; set; }
        [JsonIgnore]
        public bool IsAdmin { get; set; }
        [JsonIgnore]
        public List<int> Branches { get; set; }
        [JsonIgnore]
        public List<int> Sites { get; set; }
        [JsonIgnore]
        public List<int> ActiveBranches { get; set; }
        [JsonIgnore]
        public List<int> ActiveSites { get; set; }
        [JsonIgnore]
        public List<string> Fields { get; set; }
        [JsonIgnore]
        public List<string> PrivateMenuIDs { get; set; }
        [JsonIgnore]
        public List<string> PublicMenuIDs { get; set; }
        [JsonIgnore]
        public IDictionary<string, object> Items { get; set; }
        public void AddItem(string key, object value)
        {
            if (this.Items.ContainsKey(key))
                this.Items[key] = value;
            else
                this.Items.Add(key, value);
        }

        public Dictionary<string, bool> Rights { get; set; }

    }
}
