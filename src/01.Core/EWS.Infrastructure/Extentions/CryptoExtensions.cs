using EWS.Application.Const;
using eXtensionSharp;

namespace EWS.Infrastructure.Extentions;

public static class CryptoExtensions
{
    public static string vToAESEncrypt(this string text)
    {
        if (text.xIsEmpty()) return string.Empty;
        return text.xToAESEncrypt(ApplicationConstants.Encryption.DB_ENC_KEY,
            ApplicationConstants.Encryption.DB_ENC_IV);
    }
    
    public static string vToAESDecrypt(this string text)
    {
        if (text.xIsEmpty()) return String.Empty;
        if (text.xContains("SYSTEM")) return text;
        
        var decText = string.Empty;
        try
        {
            decText = text.xToAESDecrypt(ApplicationConstants.Encryption.DB_ENC_KEY,
                ApplicationConstants.Encryption.DB_ENC_IV);
        }
        catch
        {
            decText = text;
        }

        return decText;
    }
}