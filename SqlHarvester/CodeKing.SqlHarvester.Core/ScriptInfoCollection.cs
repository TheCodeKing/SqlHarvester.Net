using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Collections;

namespace CodeKing.SqlHarvester
{
    [ConfigurationCollection(typeof(ScriptInfo), AddItemName = "ScriptInfo")]
    public class ScriptInfoCollection : ConfigurationElementCollection, IEnumerable
    {
        public override bool IsReadOnly()
        {
            return false;
        }

        public new IScriptInfo this[string scriptInfo]
        {
            get
            {
                return (IScriptInfo)base.BaseGet(scriptInfo.ToLowerInvariant());
            }
        }

        public void Add(IScriptInfo scriptInfo)
        {
            base.BaseAdd(scriptInfo as ScriptInfo);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public bool Contains(string scriptInfo)
        {
            object value = base.BaseGet(scriptInfo.ToLowerInvariant());
            return (value != null);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        protected override string ElementName
        {
            get
            {
                return "ScriptInfo";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ScriptInfo();
        }

        protected override ConfigurationElement CreateNewElement(string scriptInfo)
        {
            return new ScriptInfo();
        }

        protected override bool IsElementName(string elementName)
        {
            return (elementName == "ScriptInfo");
        }

        protected override object GetElementKey(ConfigurationElement scriptInfo)
        {
            ScriptInfo scriptInfoObj = (ScriptInfo)scriptInfo;
            return scriptInfoObj.Name.ToLowerInvariant();
        }
    }
}
