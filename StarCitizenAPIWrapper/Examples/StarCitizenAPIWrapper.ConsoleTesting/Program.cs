using System.Threading.Tasks;
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
            var client = StarCitizenClient.GetClient();
            var user = await client.GetUser("Anachronis");
        }
    }
}
