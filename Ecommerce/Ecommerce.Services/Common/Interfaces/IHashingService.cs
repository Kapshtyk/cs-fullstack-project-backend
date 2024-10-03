namespace Ecommerce.Services.Common.Interfaces
{
    public interface IHashingService
    {
        void HashValue(string originalValue, out string hashedValue, byte[] salt);
        bool VerifyValue(string inputValue, string storedValue, byte[] salt);
        byte[] GenerateSalt();
    }
}