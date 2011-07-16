/*=============================================================================
*
*	(C) Copyright 2011, Michael Carlisle (mike.carlisle@thecodeking.co.uk)
*
*   http://www.TheCodeKing.co.uk
*  
*	All rights reserved.
*	The code and information is provided "as-is" without waranty of any kind,
*	either expressed or implied.
*
*=============================================================================
*/
using System.Collections;
using System.Configuration;

namespace CodeKing.SqlHarvester.Core
{
    [ConfigurationCollection(typeof(ScriptInfo), AddItemName = "ScriptInfo")]
    public class ScriptInfoCollection : ConfigurationElementCollection, IEnumerable
    {
        #region Properties

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

        #endregion

        #region Indexers

        public new IScriptInfo this[string scriptInfo]
        {
            get
            {
                return (IScriptInfo)base.BaseGet(scriptInfo.ToLowerInvariant());
            }
        }

        #endregion

        #region Public Methods

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

        public override bool IsReadOnly()
        {
            return false;
        }

        #endregion

        #region Methods

        protected override ConfigurationElement CreateNewElement()
        {
            return new ScriptInfo();
        }

        protected override ConfigurationElement CreateNewElement(string scriptInfo)
        {
            return new ScriptInfo();
        }

        protected override object GetElementKey(ConfigurationElement scriptInfo)
        {
            ScriptInfo scriptInfoObj = (ScriptInfo)scriptInfo;
            return scriptInfoObj.Name.ToLowerInvariant();
        }

        protected override bool IsElementName(string elementName)
        {
            return (elementName == "ScriptInfo");
        }

        #endregion
    }
}
