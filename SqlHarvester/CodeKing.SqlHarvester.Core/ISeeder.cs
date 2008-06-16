using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CodeKing.SqlHarvester
{
    public interface ISeeder : IDisposable
    {
        string[] GetFiles();

        void ImportFile(string file);
    }
}
