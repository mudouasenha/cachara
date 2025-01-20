using System.Text;
using System.Text.Json;

namespace Cachara.Shared.Infrastructure.Security;

public static class GeneralDataProtectionServiceExtensions
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public static string EncryptString(this IGeneralDataProtectionService protectionService, string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return string.Empty;
        }

        var plainBytes = Encoding.UTF8.GetBytes(data);
        var encryptedBytes = protectionService.Encrypt(plainBytes);
        var encryptedText = Convert.ToBase64String(encryptedBytes);
        return encryptedText;
    }

    public static string DecryptString(this IGeneralDataProtectionService protectionService, string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return string.Empty;
        }

        var encryptedBytes = Convert.FromBase64String(data);
        var decryptedBytes = protectionService.Decrypt(encryptedBytes);
        var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
        return decryptedText;
    }

    public static string EncryptObject<T>(this IGeneralDataProtectionService protectionService, T data)
    {
        return protectionService.EncryptString(Serialize(data));
    }

    public static T DecryptObject<T>(this IGeneralDataProtectionService protectionService, string data)
    {
        return Deserialize<T>(protectionService.DecryptString(data));
    }

    private static T Deserialize<T>(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data, jsonSerializerOptions);
    }

    private static string Serialize<T>(T data)
    {
        if (data is null)
        {
            return string.Empty;
        }

        return JsonSerializer.Serialize(data, jsonSerializerOptions);
    }
}
