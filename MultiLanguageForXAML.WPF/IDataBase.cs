namespace MultiLanguageForXAML
{
    public interface IDataBase
    {
        string? Get(string key, string cultureName);
    }
}
