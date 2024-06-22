using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Games.Application.Infrastructure;

public static class PasswordHash
{
    private static int keySize = 64;
    private static int iterations = 100_000;
    private static HashAlgorithmName algorithm = HashAlgorithmName.SHA512;

    public static (string Password, byte[] Salt) HashPasword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        var salt = RandomNumberGenerator.GetBytes(keySize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            algorithm,
            keySize);

        return (Convert.ToHexString(hash), salt);
    }

    public static bool VerifyPassword(string password, string hash, byte[] salt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
        ArgumentException.ThrowIfNullOrWhiteSpace(hash, nameof(hash));

        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithm, keySize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}
