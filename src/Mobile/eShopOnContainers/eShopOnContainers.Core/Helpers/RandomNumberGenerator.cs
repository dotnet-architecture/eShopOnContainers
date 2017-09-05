using System.Text;

namespace eShopOnContainers.Core.Helpers
{
    internal static class RandomNumberGenerator
    {
        public static string CreateUniqueId(int length = 64)
        {
            var bytes = PCLCrypto.WinRTCrypto.CryptographicBuffer.GenerateRandom(length);
            return ByteArrayToString(bytes);
        }

        private static string ByteArrayToString(byte[] array)
        {
            var hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}
