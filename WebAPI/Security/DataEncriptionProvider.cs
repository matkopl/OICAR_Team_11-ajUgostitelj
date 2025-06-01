using System.Security.Cryptography;
using System.Text;

namespace WebAPI.Security
{
    public static class DataEncriptionProvider
    {
        private static readonly byte[] Key = GenerateKey("this_is_a_secret_password");

        private static byte[] GenerateKey(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static string Encrypt(string text)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            var bytes = Encoding.UTF8.GetBytes(text);
            var encryptedBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            return Convert.ToBase64String(aes.IV) + ":" + Convert.ToBase64String(encryptedBytes);
        } 

        public static string Decrypt(string text)
        {
            var parts = text.Split(':');
            var iv = Convert.FromBase64String(parts[0]);
            var encryptedBytes = Convert.FromBase64String(parts[1]);

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public static bool IsBase64String(string text)
        {
            Span<byte> buffer = new Span<byte>(new byte[text.Length]);
            bool isBase64 = Convert.TryFromBase64String(text, buffer, out _);
            Console.WriteLine($"DEBUG: Is Base64? {isBase64}, Email -> {text}");
            return isBase64;
        }
    }
}
