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
        private dynamic data;

        public JsonDB()
        {

        }

        public JsonDB(string jsonDir)
        {
            this.jsonDir = jsonDir;
            data = null;
        }

        public Task<string> Get(string key, string cultureName)
        {
            return Task.Run(() =>
            {
                if (data == null)
                {
                    var files = Directory.GetFiles(jsonDir, cultureName);
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
                    data = JsonConvert.DeserializeObject<dynamic>(json);
                }
                string result = data[key];
                return result;
            });
        }
    }
}
