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
