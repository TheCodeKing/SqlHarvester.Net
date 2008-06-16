using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CodeKing.SqlHarvester
{
    public interface IHarvester : IDisposable
    {

        FileInfo WriteHeader();

        bool WriteContent();

        void WriteFooter();

        void Cancel();
    }
}
