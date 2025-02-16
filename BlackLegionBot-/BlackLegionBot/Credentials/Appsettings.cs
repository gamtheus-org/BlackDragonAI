using System.IO;
using System.Threading.Tasks;
using BlackLegionBot.CommandStorage;
using Newtonsoft.Json;

namespace BlackLegionBot.Credentials
{
    public class Appsettings
    {
        private const string AppsettingsFileLocation = "../appsettings.json";
        
        public BlbApiConfig BlbApi { get; set; }

        public static async Task<Appsettings> GetAppsettings()
        {
            using var sr = new StreamReader(AppsettingsFileLocation);
            return JsonConvert.DeserializeObject<Appsettings>(await sr.ReadToEndAsync());
        }

        public async Task WriteAppsettings()
        {
            await using var sw = new StreamWriter(AppsettingsFileLocation);
            await sw.WriteAsync(JsonConvert.SerializeObject(this));
        }
    }
}