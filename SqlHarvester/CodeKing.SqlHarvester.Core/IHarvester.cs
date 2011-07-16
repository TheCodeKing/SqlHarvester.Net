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
using System.IO;

namespace CodeKing.SqlHarvester.Core
{
    public interface IHarvester : IDisposable
    {
        #region Public Methods

        void Cancel();

        bool WriteContent();

        void WriteFooter();

        FileInfo WriteHeader();

        #endregion
    }
}
