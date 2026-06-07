using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlackLegionBot.Credentials
{
    public static class CredentialManager
    {
        private const string CredentialsFileName = "Credentials.json";

        public static async Task<ApiCredentials> GetCredentials()
        {
            using var sr = new StreamReader(CredentialsFileName);
            var rawText = await sr.ReadToEndAsync();
            return JsonSerializer.Deserialize<ApiCredentials>(rawText);
        }

        public static async Task StoreCredentials(ApiCredentials cred)
        {
            await using var sw = new StreamWriter(CredentialsFileName);
            await sw.WriteAsync(JsonSerializer.Serialize(cred));
        }
    }
}