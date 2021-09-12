using Tie;
using Sys.Data;
using Sys.IO;

namespace anycli
{

    public interface IApplicationConfiguration : IWorkspace
    {
        string OutputFile { get; }
        
        VAL GetValue(VAR variable);
        T GetValue<T>(string variable, T defaultValue = default);
        bool TryGetValue<T>(string variable, out T result);
        
    }
}