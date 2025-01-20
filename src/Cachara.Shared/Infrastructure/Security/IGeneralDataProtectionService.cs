namespace Cachara.Shared.Infrastructure.Security;

public interface IGeneralDataProtectionService
{
    byte[] Encrypt(byte[] bytes);
    byte[] Decrypt(byte[] bytes);
}
