using System.Security.Cryptography;
using System.Text;

namespace ContentApi.Services;

public class EncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(string key)
    {
        _key = Encoding.UTF8.GetBytes(key);
    }

    public void EncryptStream(Stream input, Stream output)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        output.Write(aes.IV, 0, aes.IV.Length);

        using var encryptor = aes.CreateEncryptor();
        using var cryptoStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write);
        input.CopyTo(cryptoStream);
    }

    public void DecryptStream(Stream input, Stream output)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        var iv = new byte[aes.IV.Length];
        input.Read(iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var cryptoStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read);
        cryptoStream.CopyTo(output);
    }
}