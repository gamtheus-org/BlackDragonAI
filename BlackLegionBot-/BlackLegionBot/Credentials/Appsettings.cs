using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;

namespace BlackLegionBot.Credentials
{
    public class Appsettings
    {
        private const string AppsettingsFileLocation = "../appsettings.json";
        
        public BlbApiConfig BlbApi { get; set; }

        public static async Task<Appsettings> GetAppsettings()
        {
            using var sr = new StreamReader(AppsettingsFileLocation);
            return JsonSerializer.Deserialize<Appsettings>(await sr.ReadToEndAsync());
        }

        public async Task WriteAppsettings()
        {
            await using var sw = new StreamWriter(AppsettingsFileLocation);
            await sw.WriteAsync(JsonSerializer.Serialize(this));
        }
    }
}