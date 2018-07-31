using System;
using System.Threading.Tasks;

namespace MultiLanguageManager
{
    public class LanService
    {
        public static LanService SharedInstance { get; private set; } = new LanService();

        public void Init(string jsonDir)
        {

        }

        public Task<string> Get(string key)
        {
            return null;
        }
    }
}
