using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Library.V1.Common;
using Library.V1.SQL;

namespace Library.V1.Entity
{
    public class LanguageHelper
    {
        static LanguageHelper()
        {
            WordBook = new Dictionary<string, Dictionary<string, string>>();
        }
        #region Private Fields
        private static string Lang { get; set; }
        private static Dictionary<string, Dictionary<string, string>> WordBook { get; set; }
        #endregion

        #region Public Methods
        public static Dictionary<string, string> GetDictionary()
        {
            if (WordBook.ContainsKey(Lang))
                return WordBook[Lang];
            else
                return new Dictionary<string, string>();
        }
        public static string Words(string keyword)
        {
            string ret = "";
            if(WordBook.ContainsKey(Lang))
            {
                if(WordBook[Lang].ContainsKey(keyword))
                    if (String.IsNullOrWhiteSpace(WordBook[Lang][keyword]))
                        ret = keyword.Replace(".", " ").Uword();
                    else
                        ret = WordBook[Lang][keyword];
                else
                    ret = keyword.Replace(".", " ").Uword();

            }
            else
            {
                ret = keyword.Replace(".", " ").Uword();
            }
            return ret;
        }
        public static void InitWords(SqlHelper dsql)
        {
            SqlHelper DSQL = dsql;
            // To prevent frenquent get translation.  any updated, we need to setup status=1 to obtain latest translation.
            List<Dictionary<string,string>> gsetLists  = DSQL.Query("SELECT ItemName, ItemValue FROM GSetting WHERE Active=1 AND Deleted=0", new Dictionary<string, object>());
            gsetLists.ForEach(p => {
                DSQL.AddSwitch(p["ItemName"], p["ItemValue"]);
            });

            if ( (DSQL.SwitchValue("Translation").GetBool()??false) || WordBook.Count==0) 
                SyncTranslation(DSQL);
            Lang = DSQL.Lang;
        }
        #endregion

        #region Private Methods
        private static void SyncTranslation(SqlHelper dsql)
        {
            SqlHelper DSQL = dsql;
            List<Dictionary<string, string>> rows = DSQL.Query("SELECT Keyword, Word_en, Word_cn FROM GTranslation WHERE Deleted=0 AND Active=1", new Dictionary<string, object>());
            WordBook.Clear();
            WordBook.Add("en", new Dictionary<string, string>());
            WordBook.Add("cn", new Dictionary<string, string>());
            foreach (var row in rows)
            {
                WordBook["en"].Add(row["Keyword"].GetString().ToLower(), row["Word_en"].GetString());
                WordBook["cn"].Add(row["Keyword"].GetString().ToLower(), row["Word_cn"].GetString());
            }
            DSQL.ExecuteQuery("Update GSetting SET ItemValue = 'False' WHERE ItemName='Translation'");
        }
        #endregion
    }
}
