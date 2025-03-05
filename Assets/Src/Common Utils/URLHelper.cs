using System;

namespace Src.Common_Utils
{
    public class URLHelper
    {
        public static string AddAntiCachePostfix(string url)
        {
            var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            var charStr = (url.IndexOf('?') > 0) ? "&" : "?";
            return $"{url}{charStr}anticache={timestamp.ToString()}";
        }
    }
}
