namespace syscon.stdio
{
    public interface ITabCompletion
    {
        string[] TabCandidates(string argument);
    }
}
