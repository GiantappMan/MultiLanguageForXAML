using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiLanguageManager
{
    public class LanService
    {
        static IDataBase _db;
        static string _currentCulture;

        public static void Init(IDataBase db)
        {
            _db = db;
            ApplyCulture();
        }

        /// <summary>
        /// 修改Culture后重新调用，刷新
        /// </summary>
        public static void ApplyCulture()
        {
#if WINDOWS_UWP
            var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var language = new Windows.Globalization.Language(topUserLanguage);
            _currentCulture = language.LanguageTag;
#else
            _currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
#endif
        }

        /// <summary>
        /// 是否启用动态切换多语言，开启后有性能损耗
        /// </summary>
        /// <param name="enable"></param>
        public static void MonitorCulture(bool enable)
        {

        }

        public static Task<string> Get(string key)
        {
            return Get(key, _currentCulture);
        }

        public static Task<string> Get(string key, string cultureName)
        {
            return _db.Get(key, cultureName);
        }
    }
}
