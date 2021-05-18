using System;
using System.Text;

public class Base64Helper
{
    public static string Base64Encode(string inputStr)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(inputStr);
        string encodedText = Convert.ToBase64String(bytesToEncode);
        return encodedText;
    }

    public static string Base64Decode(string base64Str)
    {
        byte[] decodedBytes = Convert.FromBase64String(base64Str);
        string decodedText = Encoding.UTF8.GetString(decodedBytes);
        return decodedText;
    }
}
