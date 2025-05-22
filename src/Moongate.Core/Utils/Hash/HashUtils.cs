using System.Security.Cryptography;
using System.Text;

namespace Moongate.Core.Utils.Hash;

/// <summary>
/// Provides utility methods for cryptographic operations including hashing, password management, and encryption.
/// </summary>
public static class HashUtils
{
    /// <summary>
    /// Computes a SHA-256 hash of the given string.
    /// </summary>
    /// <param name="rawData">The string to hash.</param>
    /// <returns>A hexadecimal string representation of the SHA-256 hash.</returns>
    public static string ComputeSha256Hash(string rawData)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        var builder = new StringBuilder();
        foreach (var b in bytes) builder.Append(b.ToString("x2"));

        return builder.ToString();
    }

    /// <summary>
    /// Generates a secure hash of a password using PBKDF2 with SHA-256.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>A tuple containing the base64-encoded hash and salt.</returns>
    /// <remarks>
    /// Uses 100,000 iterations of PBKDF2 with a random 16-byte salt for security.
    /// The hash is 32 bytes (256 bits) in length.
    /// </remarks>
    public static (string Hash, string Salt) HashPassword(string password)
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256))
        {
            var hash = pbkdf2.GetBytes(32); // 256-bit hash
            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }
    }

    /// <summary>
    /// Verifies a password against a stored hash and salt.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="storedHash">The base64-encoded stored hash.</param>
    /// <param name="storedSalt">The base64-encoded stored salt.</param>
    /// <returns>True if the password matches the stored hash, false otherwise.</returns>
    /// <remarks>
    /// Uses PBKDF2 with 100,000 iterations and SHA-256, matching the settings used in HashPassword.
    /// The verification is performed using a constant-time comparison to prevent timing attacks.
    /// </remarks>
    public static bool CheckPasswordHash(string password, string storedHash, string storedSalt)
    {
        var salt = Convert.FromBase64String(storedSalt);
        var expectedHash = Convert.FromBase64String(storedHash);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(hash, expectedHash);
    }

    /// <summary>
    /// Creates a formatted password hash string in the format "Hash:Salt".
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>A string in the format "Hash:Salt" where both Hash and Salt are base64-encoded.</returns>
    public static string CreatePassword(string password)
    {
        var (hash, salt) = HashPassword(password);
        return $"hash://{hash}:{salt}";
    }

    /// <summary>
    /// Verifies a password against a combined hash:salt string.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashSaltCombined">The combined hash and salt in the format "Hash:Salt".</param>
    /// <returns>True if the password matches, false otherwise.</returns>
    /// <exception cref="FormatException">Thrown when the combined hash:salt string is not in the correct format.</exception>
    public static bool VerifyPassword(string password, string hashSaltCombined)
    {
        if (hashSaltCombined.StartsWith("hash://"))
        {
            hashSaltCombined = hashSaltCombined[7..];
        }

        var parts = hashSaltCombined.Split(':');

        if (parts.Length != 2)
        {
            throw new FormatException("The hash:salt combined string is not in the correct format. Expected 'Hash:Salt'.");
        }

        var hash = parts[0];
        var salt = parts[1];

        return CheckPasswordHash(password, hash, salt);
    }

    /// <summary>
    /// Generates a cryptographically secure random string suitable for use as a refresh token.
    /// </summary>
    /// <param name="size">The size of the token in bytes, defaults to 32 (256 bits).</param>
    /// <returns>A base64-encoded random string.</returns>
    public static string GenerateRandomRefreshToken(int size = 32)
    {
        var randomBytes = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Generates a cryptographically secure random key in base64 format.
    /// </summary>
    /// <param name="byteLength">The length of the key in bytes, defaults to 32 (256 bits).</param>
    /// <returns>A base64-encoded random key.</returns>
    public static string GenerateBase64Key(int byteLength = 32)
    {
        var key = new byte[byteLength];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }

    /// <summary>
    /// Encrypts a string using AES with the provided key.
    /// </summary>
    /// <param name="plaintext">The text to encrypt.</param>
    /// <param name="key">The encryption key (should be 16, 24, or 32 bytes for AES-128, AES-192, or AES-256).</param>
    /// <returns>A byte array containing the IV concatenated with the encrypted data.</returns>
    /// <remarks>
    /// A random IV (Initialization Vector) is generated for each encryption operation.
    /// The IV is prepended to the ciphertext in the returned array (first 16 bytes).
    /// </remarks>
    public static byte[] Encrypt(string plaintext, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var cipherBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        // concatena IV + CIPHERTEXT
        return aes.IV.Concat(cipherBytes).ToArray();
    }

    /// <summary>
    /// Decrypts AES-encrypted data using the provided key.
    /// </summary>
    /// <param name="ivAndCiphertext">A byte array containing the IV (first 16 bytes) concatenated with the encrypted data.</param>
    /// <param name="key">The decryption key (should match the key used for encryption).</param>
    /// <returns>The decrypted plaintext as a string.</returns>
    /// <remarks>
    /// Expects the IV to be the first 16 bytes of the input array, followed by the ciphertext.
    /// This format matches the output of the Encrypt method.
    /// </remarks>
    public static string Decrypt(byte[] ivAndCiphertext, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;

        aes.IV = ivAndCiphertext[..16];
        var cipher = ivAndCiphertext[16..];

        using var decryptor = aes.CreateDecryptor();
        var plaintextBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
