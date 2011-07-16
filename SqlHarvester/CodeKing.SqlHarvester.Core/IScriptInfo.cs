namespace CodeKing.SqlHarvester.Core
{
    public interface IScriptInfo
    {
        #region Properties

        string Filter { get; set; }

        string Name { get; set; }

        string QualifiedName { get; }

        ScriptMode ScriptMode { get; set; }

        #endregion
    }
}
