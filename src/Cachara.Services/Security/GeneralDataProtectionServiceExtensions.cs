using System.Text;
using System.Text.Json;
using Cachara.Domain.Abstractions.Security;

namespace Cachara.Services.Security;

    public static class GeneralDataProtectionServiceExtensions
    {
        public static string EncryptString(this IGeneralDataProtectionService protectionService, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
    
            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            var encryptedBytes = protectionService.Encrypt(plainBytes);
            string encryptedText = Convert.ToBase64String(encryptedBytes);
            return encryptedText;
        }
    
        public static string DecryptString(this IGeneralDataProtectionService protectionService, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
    
            byte[] encryptedBytes = Convert.FromBase64String(data);
            byte[] decryptedBytes = protectionService.Decrypt(encryptedBytes);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedText;
        }
        
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    
        public static string EncryptObject<T>(this IGeneralDataProtectionService protectionService, T data)
            => protectionService.EncryptString(Serialize(data));
    
        public static T DecryptObject<T>(this IGeneralDataProtectionService protectionService, string data)
            => Deserialize<T>(protectionService.DecryptString(data));
    
        private static T Deserialize<T>(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return default;
    
            return JsonSerializer.Deserialize<T>(data, jsonSerializerOptions);
        }
    
        private static string Serialize<T>(T data)
        {
            if (data is null)
                return string.Empty;
    
            return JsonSerializer.Serialize(data, jsonSerializerOptions);
        }
    }