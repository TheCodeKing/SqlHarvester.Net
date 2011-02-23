using System.Configuration;
using System.Text.RegularExpressions;

namespace CodeKing.SqlHarvester.Core
{
    public class ScriptInfo : ConfigurationElement, IScriptInfo
    {
        #region Constructors and Destructors

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
                ScriptMode = ScriptMode.NoDelete;
            }
            else if (tableArgs.ToLowerInvariant().EndsWith(" with delete"))
            {
                int z = tableArgs.ToLowerInvariant().LastIndexOf(" with delete");
                tableArgs = tableArgs.Substring(0, z).Trim();
                ScriptMode = ScriptMode.Delete;
            }
            else
            {
                ScriptMode = ScriptMode.NotSet;
            }

            int i = tableArgs.ToLower().IndexOf(" where ");
            if (i > -1)
            {
                Name = Regex.Replace(
                    tableArgs.Substring(0, i), @"(\[)|([\.\]])|(.*?\.)|(\.)", "", RegexOptions.IgnoreCase);
                Filter = tableArgs.Substring(i + 1, tableArgs.Length - (i + 1)).Trim();
            }
            else
            {
                Name = Regex.Replace(tableArgs, @"(\[)|([\.\]])|(.*?\.)|(\.)", "", RegexOptions.IgnoreCase);
            }
        }

        public ScriptInfo(string tableName, string filter, ScriptMode scriptMode)
        {
            Name = Regex.Replace(tableName, @"(\[)|([\.\]])|(.*?\.)|(\.)", "", RegexOptions.IgnoreCase);
            Filter = filter;
            ScriptMode = scriptMode;
        }

        #endregion

        #region Properties

        [ConfigurationProperty("filter", DefaultValue = "")]
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

        #endregion

        #region Public Methods

        public override bool IsReadOnly()
        {
            return false;
        }

        #endregion
    }
}
