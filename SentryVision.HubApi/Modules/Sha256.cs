using System.Security.Cryptography;
using System.Text;

namespace SentryVision.HubApi.Modules;

public class Sha256
{
    public static string GenerateSha256(string text)
    {
        using (var sha256 = new SHA256Managed())
        {
            var lower = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "").ToLower();
            return lower;
        }
    }
}