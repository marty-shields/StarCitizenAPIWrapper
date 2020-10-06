using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarCitizenAPIWrapper.Library;
using StarCitizenAPIWrapper.Library.Helpers;

namespace StarCitizenAPIWrapper.Example.Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection()
                .AddSingleton(config)
                .AddHttpClient()
                .AddStarCitizenClient()
                .BuildServiceProvider();

            var client = services.GetService<StarCitizenClient>();
            var result = await client.GetUser("Anachronis");
        }
    }
}
