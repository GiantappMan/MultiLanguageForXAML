using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MultiLanguageManager
{
    public class JsonDB : IDataBase
    {
        private string jsonDir;
        private Dictionary<string, dynamic> dataDict = new Dictionary<string, dynamic>();

        public JsonDB()
        {

        }

        public JsonDB(string jsonDir)
        {
            this.jsonDir = jsonDir;
        }

        public Task<string> Get(string key, string cultureName)
        {
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
                var data = JsonConvert.DeserializeObject<dynamic>(json);
                dataDict.Add(cultureName, data);
            }

            string result = dataDict[cultureName][key];
            return Task.FromResult(result);
        }
    }
}
