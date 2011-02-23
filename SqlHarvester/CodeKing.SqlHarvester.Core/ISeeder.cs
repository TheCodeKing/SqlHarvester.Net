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
