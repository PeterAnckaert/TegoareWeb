using System;
using System.Security.Cryptography;

namespace TegoareWeb.Data
{
    public static class Crypto
    {
        // de salt bestaat uit 24 bytes
        private const int SaltSize = 24;

        // de hash van het wachtwoord+salt bestaat uit 24 bytes
        private const int HashSize = 24;

        // aantal maal dat er geïtereerd moet worden om de hash te bekomen
        private const int IterationSize = 101010;

        // wachtwoord + salt ==> hashed wachtwoord
        public static string Hash(string password)
        {
            // krijg een willekeurige salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // hash wachtwoord+salt 
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationSize);
            var hash = pbkdf2.GetBytes(HashSize);

            // plaats de salt bij de hash (zodat we later nog weten welke salt
            // we gebruikt hebben)
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool Verify(string password, string hashedPassword)
        {
            // indien één of beide null zijn, kunnen we niet vergelijken
            if(password == null || hashedPassword == null)
            {
                return false;
            }
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // haal de salt uit de hashedPassword
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // hash wachtwoord+salt 
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationSize);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // controleer of de nieuwe hash gelijk is aan de hash in hasedPassword 
            for (var i = 0; i < HashSize; i++)
            {
                // er is een verschil, dus niet identiek
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
