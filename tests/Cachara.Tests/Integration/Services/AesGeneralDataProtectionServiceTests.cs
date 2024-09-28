using System.Security.Cryptography;
using System.Text;
using AutoFixture;
using Cachara.Services.Security;

namespace Cachara.Tests.Integration.Services;

public class AesGeneralDataProtectionServiceTests
{
    private readonly IFixture _fixture = new Fixture();
    private readonly AesGeneralDataProtectionService _aes256Encryptor;

    public AesGeneralDataProtectionServiceTests()
    {
        var key = GenerateRandomKey();
        _aes256Encryptor = new AesGeneralDataProtectionService(Convert.ToBase64String(key));
    }
    
    [Trait("Category", "Integration")]
    [Trait("Feature", "Security")]
    [Trait("Algorithm", "AES256")]
    [Fact(DisplayName = "Encrypt Then Decrypt Should Return Original Plaintext")]
    public void Encrypt_Decrypt_ShouldReturnOriginalPlaintext()
    {
        // Arrange
        var plaintext = _fixture.Create<string>();
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        // Act 
        var encryptedBytes = _aes256Encryptor.Encrypt(plaintextBytes);
        var decryptedBytes = _aes256Encryptor.Decrypt(encryptedBytes);

        // Assert
        var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
        Assert.Equal(plaintext, decryptedText);
    }
    
    [Trait("Category", "Integration")]
    [Trait("Feature", "Security")]
    [Trait("Algorithm", "AES256")]
    [Fact(DisplayName = "Encrypt Should Return Different Output For Different Messages")]
    public void Encrypt_ShouldReturnDifferentOutputForDifferentMessages()
    {
        // Arrange
        var plaintext1 = _fixture.Create<string>();
        var plaintext2 = _fixture.Create<string>();
        
        var plaintextBytes1 = Encoding.UTF8.GetBytes(plaintext1);
        var plaintextBytes2 = Encoding.UTF8.GetBytes(plaintext2);

        // Act
        var encryptedBytes1 = _aes256Encryptor.Encrypt(plaintextBytes1);
        var encryptedBytes2 = _aes256Encryptor.Encrypt(plaintextBytes2);

        // Assert
        Assert.NotEqual(encryptedBytes1, encryptedBytes2);
    }
    
    private static byte[] GenerateRandomKey(int size = 32)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var key = new byte[size];
            rng.GetBytes(key);
            return key;
        }
    }
}
