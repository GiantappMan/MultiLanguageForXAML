using System;
using System.Threading;

namespace MultiLanguageForXAML
{
    public class LanService
    {
        static IDataBase? _db;
        static string? _currentCulture;
        static bool _canHotUpdate;
        static string? _defaultCulture;

        public static bool CanHotUpdate { get => _canHotUpdate; }
        public static Exception? LastError { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="hotUpdate">是否支持热更新，true会有一定性能开销</param>
        /// <param name="currentCulture">当前多语言</param>
        /// <param name="defaultCulture">当目标多语言不存在时，使用的默认Culture</param>
        public static void Init(IDataBase db, bool hotUpdate = false, string? currentCulture = null, string? defaultCulture = null)
        {
            _defaultCulture = defaultCulture;
            _canHotUpdate = hotUpdate;
            _db = db;
            _currentCulture = currentCulture ?? GetCurrentUICulture();
        }

        /// <summary>
        /// 修改多语言
        /// </summary>
        public static void UpdateCulture(string? culture)
        {
            if (!CanHotUpdate)
                return;

            _currentCulture = culture ?? GetCurrentUICulture();
            Xaml.UpdateLanguage();
        }

        public static string? Get(string key)
        {
            if (_currentCulture == null)
                return null;
            return Get(key, _currentCulture);
        }

        public static string? Get(string key, string cultureName)
        {
            if (_db == null)
                throw new NullReferenceException("Language database has not been initialized");
            string? r;
            try
            {
                r = _db.Get(key, cultureName);
                if (!string.IsNullOrEmpty(r))
                    return r;
            }
            catch (Exception ex)
            {
                LastError = ex;
            }

            if (_defaultCulture == null)
                return null;

            //出现异常或者没有多语言，使用默认语言
            r = _db.Get(key, _defaultCulture);
            return r;
        }

        private static string GetCurrentUICulture()
        {
            string result = Thread.CurrentThread.CurrentUICulture.Name;
            return result;
        }
    }
}
