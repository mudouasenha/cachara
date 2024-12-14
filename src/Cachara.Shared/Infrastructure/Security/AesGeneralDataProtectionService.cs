using System.Security.Cryptography;

namespace Cachara.Shared.Infrastructure.Security;

// TODO: Implement AES256 encryption.
public class AesGeneralDataProtectionService : IGeneralDataProtectionService
{
    private readonly byte[] _key;
    
    public AesGeneralDataProtectionService(string key)
    {
        this._key =  Convert.FromBase64String(key);
    }
    public byte[] Encrypt(byte[] bytes)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV(); // Generate a random IV for every encryption operation

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        // Prepend the IV to the output
        memoryStream.Write(aes.IV, 0, aes.IV.Length);

        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
        }

        return memoryStream.ToArray();
    }

    public byte[] Decrypt(byte[] bytes)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Extract the IV from the input (the first 16 bytes)
        var iv = new byte[16];
        Array.Copy(bytes, 0, iv, 0, iv.Length);

        using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
        using (var memoryStream = new MemoryStream(bytes, iv.Length, bytes.Length - iv.Length))
        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
        using (var resultStream = new MemoryStream())
        {
            cryptoStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }
}