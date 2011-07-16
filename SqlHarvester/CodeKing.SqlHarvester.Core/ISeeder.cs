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
using System;

namespace CodeKing.SqlHarvester.Core
{
    public interface ISeeder : IDisposable
    {
        #region Public Methods

        string[] GetFiles();

        void ImportFile(string file);

        #endregion
    }
}
