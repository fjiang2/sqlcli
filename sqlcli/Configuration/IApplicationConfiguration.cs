using Tie;
using Sys.Data;
using Sys.IO;

namespace sqlcli
{

    public interface IApplicationConfiguration : IWorkSpace
    {
        string OutputFile { get; }
        string XmlDbDirectory { get; }
        int TopLimit { get; }
        int MaxRows { get; }
        VAL GetValue(VAR variable);
        T GetValue<T>(string variable, T defaultValue = default);
        bool TryGetValue<T>(string variable, out T result);
        IConnectionConfiguration Connection { get; }
    }
}