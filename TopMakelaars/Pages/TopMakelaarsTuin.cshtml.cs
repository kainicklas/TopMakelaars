using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TopMakelaars.Models;

namespace TopMakelaars.Pages
{
    public class TopMakelaarsTuinModel : PageModel
    {
        private readonly IDataAggregator _dataAggregator;
        public TopMakelaarsTuinModel(IDataAggregator dataAggregator)
        {
            _dataAggregator = dataAggregator;
        }
        public RealEstateAgent[] RealEstateAgentsTop10 { get; set; }



        public async Task OnGetAsync()
        {
            HttpContext.Session.SetString("dummy", "dummyvalue"); //Keep the session id between requests
            var fundaPartnerApiDataDirectoryPath = Path.Combine(Path.GetTempPath(), "fundaPartnerApiData");
            Directory.CreateDirectory(fundaPartnerApiDataDirectoryPath);
            var filePath = Path.Combine(fundaPartnerApiDataDirectoryPath,
                                        $"TopRealEstateAgents_gardenObjects_{HttpContext.Session.Id}");

            if (System.IO.File.Exists(filePath))
            {
                var topListStr = System.IO.File.ReadAllText(filePath);
                RealEstateAgentsTop10 = JsonConvert.DeserializeObject<RealEstateAgent[]>(topListStr);
            }
            else
            {
                var task = _dataAggregator.GetTopRealEstateAgentsAsync("tuin");
                task.ContinueWith((t) =>
                {
                    using (var sw = new StreamWriter(filePath, false))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(t.Result));
                    }
                }

                );
            };
        }
    }
}