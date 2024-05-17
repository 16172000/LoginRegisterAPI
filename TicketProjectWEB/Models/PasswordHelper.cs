using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    //public static string GenerateSalt()
    //{
    //    // Generate a random salt
    //    byte[] saltBytes = new byte[16];
    //    using (var rngCsp = new RNGCryptoServiceProvider())
    //    {
    //        rngCsp.GetBytes(saltBytes);
    //    }
    //    return Convert.ToBase64String(saltBytes);
    //}

    public static string GenerateSalt()
    {
        // Generate a random salt
        byte[] saltBytes = new byte[16];
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    public static string GeneratePasswordHash(string password, string salt)
    {
        if (string.IsNullOrEmpty(salt))
        {
            throw new ArgumentException("Salt cannot be null or empty.", nameof(salt));
        }

        using (var sha256 = SHA256.Create())
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];

            // Concatenate the salt and password bytes
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

            byte[] hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }


    public static bool VerifyPassword(string enteredPassword, string storedPasswordHash, string salt)
    {
        string enteredPasswordHash = GeneratePasswordHash(enteredPassword, salt);
        return storedPasswordHash == enteredPasswordHash;
    }

    //public static bool VerifyPassword(string enteredPassword, string storedPasswordHash, string salt)
    //{
    //    // Check for null or empty salt
    //    if (string.IsNullOrEmpty(salt))
    //    {
    //        return false;
    //    }

    //    // Generate the hash of the entered password with the same salt
    //    string enteredPasswordHash = GeneratePasswordHash(enteredPassword, salt);

    //    // Compare the entered password hash with the stored password hash
    //    return string.Equals(enteredPasswordHash, storedPasswordHash);
    //}
}
