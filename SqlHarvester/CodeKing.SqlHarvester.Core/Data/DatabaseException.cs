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

namespace CodeKing.SqlHarvester.Core.Data
{
    /// <summary>
    /// The exception which occurs when the Cms fails to access the database.
    /// </summary>
    public class DatabaseException : ApplicationException
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DatabaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseException"/> class.
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="args">The args.</param>
        public DatabaseException(string formatString, params string[] args)
            : base(string.Format(formatString, args))
        {
        }

        #endregion
    }
}
