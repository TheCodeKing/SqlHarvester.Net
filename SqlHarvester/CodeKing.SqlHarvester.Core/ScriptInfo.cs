using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;

namespace CodeKing.SqlHarvester
{
    public class ScriptInfo : ConfigurationElement, IScriptInfo
    {
        public override bool IsReadOnly()
        {
            return false;
        }

        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return base["name"] as string;
            }
            set
            {
                base["name"] = value;
            }
        }

        public string QualifiedName
        {
            get
            {
                return string.Concat("[dbo].[", Name, "]");
            }
        }

        [ConfigurationProperty("filter", DefaultValue="")]
        public string Filter
        {
            get
            {
                string filter = base["filter"] as string;
                if (!string.IsNullOrEmpty(filter))
                {
                    filter = Regex.Replace(filter.Trim(), @"^where\s", "", RegexOptions.IgnoreCase).Trim();
                    return filter;
                }
                return string.Empty;
            }
            set
            {
                base["filter"] = value;
            }
        }

        [ConfigurationProperty("scriptMode", DefaultValue = ScriptMode.NotSet)]
        public ScriptMode ScriptMode
        {
            get
            {
                return (ScriptMode)base["scriptMode"];
            }
            set
            {
                base["scriptMode"] = value;
            }
        }
       
        public ScriptInfo()
        {
        }

        public ScriptInfo(string tableArgs)
        {
            tableArgs = tableArgs.Trim();
            if (tableArgs.ToLowerInvariant().EndsWith(" with nodelete"))
            {
                int z = tableArgs.ToLowerInvariant().LastIndexOf(" with nodelete");
                tableArgs = tableArgs.Substring(0, z).Trim();
                this.ScriptMode = ScriptMode.NoDelete;
            }
            else if (tableArgs.ToLowerInvariant().EndsWith(" with delete"))
            {
                int z = tableArgs.ToLowerInvariant().LastIndexOf(" with delete");
                tableArgs = tableArgs.Substring(0, z).Trim();
                this.ScriptMode = ScriptMode.Delete;
            }
            else
            {
                this.ScriptMode = ScriptMode.NotSet;
            }

            int i = tableArgs.ToLower().IndexOf(" where ");
            if (i > -1)
            {
                this.Name = Regex.Replace(tableArgs.Substring(0, i), @"(\[)|([\.\]])|(.*?\.)|(\.)", "", RegexOptions.IgnoreCase);
                this.Filter = tableArgs.Substring(i + 1, tableArgs.Length - (i + 1)).Trim();
            }
            else
            {
                this.Name = Regex.Replace(tableArgs, @"(\[)|([\.\]])|(.*?\.)|(\.)", "", RegexOptions.IgnoreCase);
            }
        }

        public ScriptInfo(string tableName, string filter, ScriptMode scriptMode)
        {
            this.Name = Regex.Replace(tableName, @"(\[)|([\.\]])|(.*?\.)|(\.)", "", RegexOptions.IgnoreCase);
            this.Filter = filter;
            this.ScriptMode = scriptMode;
        }
    }
}
