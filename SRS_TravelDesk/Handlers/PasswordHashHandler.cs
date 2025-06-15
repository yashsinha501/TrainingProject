using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace SRS_TravelDesk.Handlers

{
    public class PasswordHashHandler
    {
        private static int _iterationCount = 100000;
        private static RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();


        public static string HashPassword(string password)
        {
            int saltSize = 128 / 8;
            var salt = new byte[saltSize];
            _randomNumberGenerator.GetBytes(salt);
            var subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, _iterationCount, 256 / 8);
            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01;
            
            WriteNetworkByteOrder(outputBytes, 1, (uint)KeyDerivationPrf.HMACSHA512);
            WriteNetworkByteOrder(outputBytes, 5, (uint)_iterationCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return Convert.ToBase64String(outputBytes);

        }

        public static bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var decoded = Convert.FromBase64String(hashedPassword);

            if (decoded[0] != 0x01)
                return false; // unsupported format

            var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decoded, 1);
            var iterations = (int)ReadNetworkByteOrder(decoded, 5);
            var saltLength = (int)ReadNetworkByteOrder(decoded, 9);

            var salt = new byte[saltLength];
            Buffer.BlockCopy(decoded, 13, salt, 0, saltLength);

            var expectedSubkey = new byte[decoded.Length - 13 - saltLength];
            Buffer.BlockCopy(decoded, 13 + saltLength, expectedSubkey, 0, expectedSubkey.Length);

            var actualSubkey = KeyDerivation.Pbkdf2(
                providedPassword,
                salt,
                prf,
                iterations,
                expectedSubkey.Length);

            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value);
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                 | ((uint)(buffer[offset + 1]) << 16)
                 | ((uint)(buffer[offset + 2]) << 8)
                 | ((uint)(buffer[offset + 3]));
        }

    }

}