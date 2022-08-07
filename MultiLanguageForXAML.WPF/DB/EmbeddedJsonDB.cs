using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiLanguageForXAML.DB
{
#if NET46
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    public class EmbeddedJsonDB : IDataBase
    {
        private readonly string? basePath;
        private readonly Dictionary<string, JObject?> dataDict = new();

        public EmbeddedJsonDB()
        {

        }

        public EmbeddedJsonDB(string basePath)
        {
            this.basePath = basePath;
        }

        public string? Get(string key, string cultureName)
        {
            if (basePath == null)
                return null;

            if (!dataDict.ContainsKey(cultureName))
            {
                var files = GetEmbeddedJsonFile(basePath, cultureName);
                //找不到匹配的，找近似的。例如 zh-CHS找不到,zh也可以
                if (files.Length == 0)
                {
                    bool isSubLan = cultureName.Split('-').Length > 1;
                    if (isSubLan)
                        files = GetEmbeddedJsonFile(basePath, $"{cultureName.Split('-')[0]}*");
                }

                if (files.Length == 0)
                    return null;

                string json = GetEmbeddedResource(files[0]);
                var data = JsonConvert.DeserializeObject<JObject>(json);
                dataDict.Add(cultureName, data);
            }

            string result = dataDict[cultureName]?[key]?.ToString() ?? string.Empty;
            return result;
        }

        private static string[] GetEmbeddedJsonFile(string basePath, string cultureName)
        {
            var files = Assembly.GetEntryAssembly()!.GetManifestResourceNames().Where(m => m.StartsWith($"{basePath}.{cultureName}")).ToArray();
            return files;
        }

        public static string GetEmbeddedResource(string path)
        {
            try
            {
                using var reader = new StreamReader(Assembly.GetEntryAssembly()!.GetManifestResourceStream(path)!);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
#endif

#if NETCOREAPP
    using System.Text.Json;
    public class EmbeddedJsonDB : IDataBase
    {
        private readonly string? basePath;
        private readonly Dictionary<string, JsonElement> dataDict = new();

        public EmbeddedJsonDB()
        {

        }

        public EmbeddedJsonDB(string basePath)
        {
            this.basePath = basePath;
        }

        public string? Get(string key, string cultureName)
        {
            if (basePath == null)
                return null;

            if (!dataDict.ContainsKey(cultureName))
            {
                var files = GetEmbeddedJsonFile(basePath, cultureName);
                //找不到匹配的，找近似的。例如 zh-CHS找不到,zh也可以
                if (files.Length == 0)
                {
                    bool isSubLan = cultureName.Split('-').Length > 1;
                    if (isSubLan)
                        files = GetEmbeddedJsonFile(basePath, $"{cultureName.Split('-')[0]}*");
                }

                if (files.Length == 0)
                    return null;

                string json = GetEmbeddedResource(files[0]);
                JsonElement data = JsonSerializer.Deserialize<JsonElement>(json);
                dataDict.Add(cultureName, data);
            }

            string result = dataDict[cultureName].GetProperty(key).GetString()!;
            return result;
        }

        private static string[] GetEmbeddedJsonFile(string basePath, string cultureName)
        {
            var files = Assembly.GetEntryAssembly()!.GetManifestResourceNames().Where(m => m.StartsWith($"{basePath}.{cultureName}")).ToArray();
            return files;
        }

        public static string GetEmbeddedResource(string path)
        {
            try
            {
                using var reader = new StreamReader(Assembly.GetEntryAssembly()!.GetManifestResourceStream(path)!);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
#endif
}
