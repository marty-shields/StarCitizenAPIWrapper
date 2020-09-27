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
            var client = StarCitizenClient.GetClient("12794c6b066ad75b4f73c1134ac62788");
            var org = await client.GetOrganizationMembers("GRI");
        }
    }
}
