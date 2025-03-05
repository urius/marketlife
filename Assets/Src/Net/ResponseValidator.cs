using Src.Common_Utils;

namespace Src.Net
{
    public static class ResponseValidator
    {
        public static bool Validate(CommonResponseDto responseDto)
        {
            return Validate(responseDto.response, responseDto.hash);
        }

        public static bool Validate(string response, string hash)
        {
            return hash == MD5Helper.MD5Hash(response);
        }
    }
}
