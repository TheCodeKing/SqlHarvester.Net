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
using System.Configuration;

namespace CodeKing.SqlHarvester.Core.Configuration
{
    /// <summary>
    /// The application settings which takes default values for the configurration file, but
    /// may be overriden at runtime with commandline args in some cases.
    /// delpoy service.
    /// </summary>
    public class HarvestConfigurationSection : ConfigurationSection
    {
        #region Indexers

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value></value>
        public new string this[string key]
        {
            get
            {
                ConfigurationProperty prop = GetConfigurationProperty(key);
                if (prop != null)
                {
                    return base[prop] as string;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (key.StartsWith("config") || key.StartsWith("lock"))
                {
                    return;
                }
                ConfigurationProperty found = GetConfigurationProperty(key);
                if (found != null)
                {
                    if (value.GetType() != found.Type)
                    {
                        throw new ConfigurationErrorsException(
                            string.Format(
                                "type mismatch setting {0}={1}, value is not of type {2}", key, value, found.Type.Name));
                    }
                    base[found] = value;
                }
                else
                {
                    key = key.ToLowerInvariant();
                    base.Properties.Add(new ConfigurationProperty(key, typeof(string)));
                    base[key] = (value);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            if (GetConfigurationProperty(key) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement"></see> object is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElement"></see> object is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the configuration property.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected ConfigurationProperty GetConfigurationProperty(string key)
        {
            ConfigurationProperty found = null;
            foreach (ConfigurationProperty prop in base.Properties)
            {
                if (prop.Name.ToLowerInvariant() == key.ToLowerInvariant())
                {
                    found = prop;
                }
            }
            return found;
        }

        /// <summary>
        /// Gets the configuration value without first verifying that the value exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected object GetConfigurationValue(string key)
        {
            return base[key];
        }

        /// <summary>
        /// Sets the configuration value without first verifying that the value exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void SetConfigurationValue(string key, object value)
        {
            base[key] = value;
        }

        #endregion
    }
}
