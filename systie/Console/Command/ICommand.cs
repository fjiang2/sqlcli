namespace syscon.stdio.Cli
{
    public interface ICommand
    {
        string Action { get; }
        string Arg1 { get; }
        bool? GetBoolean(string name);
        bool GetBoolean(string name, bool defaultValue);
        double? GetDouble(string name);
        double GetDouble(string name, double defaultValue);
        T GetEnum<T>(string name, T defaultValue) where T : struct;
        int? GetInt32(string name);
        int GetInt32(string name, int defaultValue);
        string GetValue(string name);
        string[] GetStringArray(string name);
        bool Has(string name);
    }
}