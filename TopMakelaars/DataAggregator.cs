using System.Linq;
using System.Threading.Tasks;
using TopMakelaars.Models;

namespace TopMakelaars
{
    public class DataAggregator : IDataAggregator
    {
        private readonly IApiClient _apiClient;

        public DataAggregator(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<RealEstateAgent[]> GetTopRealEstateAgentsAsync(string searchString)
        {
            var realEstates = await _apiClient.GetRealEstatesAsync(searchString);

            var top10 = realEstates.Where(r => !r.Sold)
                                   .GroupBy(r => r.RealEstateAgentId)
                                   .Select(rg => new RealEstateAgent()
                                   {
                                       Id = rg.Key,
                                       Name = rg.First().RealEstateAgentName,
                                       RealEstateCount = rg.Count()
                                   }).OrderByDescending(a => a.RealEstateCount).Take(10).ToArray();

            return top10;
        }
    }

    public interface IDataAggregator
    {
         Task<RealEstateAgent[]> GetTopRealEstateAgentsAsync(string searchString);
    }
}
