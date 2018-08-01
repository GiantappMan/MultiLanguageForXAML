using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MultiLanguageManager
{
    public class LanService
    {
        static IDataBase _db;
        static string _currentCulture;
        static bool _canHotUpdate;

        public static bool CanHotUpdate { get => _canHotUpdate; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="hotUpdate">是否支持热更新，true会有一定性能开销</param>
        public static void Init(IDataBase db, bool hotUpdate = false)
        {
            _canHotUpdate = hotUpdate;
            _db = db;
            _currentCulture = GetCultureName();
        }

        /// <summary>
        /// 修改Culture后重新调用，刷新
        /// </summary>
        public static async Task UpdateLanguage()
        {
            if (!CanHotUpdate)
                return;

            _currentCulture = GetCultureName();
            await Xaml.UpdateLanguage();
        }

        public static Task<string> Get(string key)
        {
            return Get(key, _currentCulture);
        }

        public static async Task<string> Get(string key, string cultureName)
        {
            var r = await _db.Get(key, cultureName);
            return r;
        }
        
        private static string GetCultureName()
        {
            string result = null;
#if WINDOWS_UWP
            var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var language = new Windows.Globalization.Language(topUserLanguage);
            result = language.LanguageTag;
#else
            result = Thread.CurrentThread.CurrentUICulture.Name;
#endif
            return result;
        }
    }
}
