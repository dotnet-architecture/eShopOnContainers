using System.Linq;
using System.Text;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus;

public static class Utils
{
    public static string CalculateMd5Hash(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes);
    }
}