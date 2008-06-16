using System;

namespace CodeKing.SqlHarvester
{
    public interface IScriptInfo
    {
        string Filter { get; set; }
        string Name { get; set; }
        string QualifiedName { get; }
        ScriptMode ScriptMode { get; set; }
    }
}
