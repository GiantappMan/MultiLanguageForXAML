using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MultiLanguageForXAML.DB
{
    public class JsonFileDB : IDataBase
    {
        private readonly string? jsonDir;
        private readonly Dictionary<string, JsonElement> dataDict = new();

        public JsonFileDB()
        {

        }

        public JsonFileDB(string jsonDir)
        {
            this.jsonDir = jsonDir;
        }

        public string? Get(string key, string cultureName)
        {
            if (jsonDir == null)
                return null;

            if (!dataDict.ContainsKey(cultureName))
            {
                var files = Directory.GetFiles(jsonDir, $"{cultureName}.json");
                //找不到匹配的，找近似的。例如 zh-CHS找不到,zh也可以
                if (files.Length == 0)
                {
                    bool isSubLan = cultureName.Split('-').Length > 1;
                    if (isSubLan)
                        files = Directory.GetFiles(jsonDir, $"{cultureName.Split('-')[0]}*");
                }

                if (files.Length == 0)
                    return null;

                string json = File.ReadAllText(files[0]);
                JsonElement data = JsonSerializer.Deserialize<JsonElement>(json);
                dataDict.Add(cultureName, data);
            }

            string? result = dataDict[cultureName].GetProperty(key).GetString();
            return result;
        }
    }
}
