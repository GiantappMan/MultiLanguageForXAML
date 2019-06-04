using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultiLanguageForXAML
{
    public interface IDataBase
    {
        Task<string> Get(string key, string cultureName);
    }
}
