using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StarCitizenAPIWrapper.Library;

namespace StarCitizenAPIWrapper.ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var client = StarCitizenClient.GetClient(config.GetSection("ApiKey").Value);

            var result = await client.GetRoadmap(RoadmapTypes.StarCitizen, "3.10");
        }
    }
}
