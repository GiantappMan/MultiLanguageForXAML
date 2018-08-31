using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

#if WINDOWS_UWP
using Windows.Globalization;

#else

#endif

namespace MultiLanguageManager
{
    public class LanService
    {
        static IDataBase _db;
        static string _currentCulture;
        static bool _canHotUpdate;
        static string _defaultLan;

        public static bool CanHotUpdate { get => _canHotUpdate; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="hotUpdate">是否支持热更新，true会有一定性能开销</param>
        public static void Init(IDataBase db, bool hotUpdate = false, string defaultLan = null)
        {
            _defaultLan = defaultLan;
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
            if (_db == null)
                throw new NullReferenceException("Language database has not been initialized");
            try
            {
                var r = await _db.Get(key, cultureName);
                return r;
            }
            catch (Exception)
            {
                var r = await _db.Get(key, _defaultLan);
                return r;
            }
        }

        private static string GetCultureName()
        {
            string result = null;
#if WINDOWS_UWP
          
            result = ApplicationLanguages.PrimaryLanguageOverride;
            if (string.IsNullOrEmpty(result))
            {
                var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
                var language = new Windows.Globalization.Language(topUserLanguage);
                result = language.LanguageTag;
            }
#else
            result = Thread.CurrentThread.CurrentUICulture.Name;
#endif
            return result;
        }
    }
}
