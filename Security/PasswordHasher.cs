using System.Security.Cryptography;

namespace SentinelAPI.Security;

public class PasswordHasher
{
    private const int Iterations = 600_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    private readonly Lazy<string> _dummy;

    public PasswordHasher() => _dummy = new Lazy<string>(() => Hash("not-a-real-password"));

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"pbkdf2-sha256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string stored)
    {
        var parts = stored.Split('$');
        if (parts.Length != 4 || parts[0] != "pbkdf2-sha256" || !int.TryParse(parts[1], out var iterations))
            return false;
        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    // Spend the same time as a real check, so unknown usernames can't be detected by timing.
    public void BurnTime(string password) => Verify(password, _dummy.Value);
}