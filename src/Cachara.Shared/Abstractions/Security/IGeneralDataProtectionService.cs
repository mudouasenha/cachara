namespace Cachara.Domain.Abstractions.Security;

public interface IGeneralDataProtectionService
{
    byte[] Encrypt(byte[] bytes);
    byte[] Decrypt(byte[] bytes);
}