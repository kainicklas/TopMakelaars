using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TopMakelaars.Models;

namespace TopMakelaars
{
    public class ApiClient : IApiClient
    {
        private HttpClient _httpClient;

        public ApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FundaPartnerApi");
        }
        public async Task<RealEstate[]> GetRealEstatesAsync(string  searchString)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var realEstates = new List<RealEstate>();
            int page = 1;
            var pageContent = new RealEstate[0];

            do
            {
                pageContent = await GetRealEstatesPerPage(page, searchString);
                realEstates.AddRange(pageContent);
                page++;
                Debug.WriteLine(page + " " + pageContent.Length);              

           } while (pageContent.Length > 0);
           //} while (page < 150);

            stopWatch.Stop();
            Debug.WriteLine($"Time elapsed{stopWatch.Elapsed}");

            return realEstates.ToArray();
        }

        private async Task<RealEstate[]> GetRealEstatesPerPage(int page, string searchString)
        {
            int pageSize = 25; //is maximum page size
            //int pageSize = 1; 
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync($"?type=koop&zo=/amsterdam/{searchString}/&page={page}&pagesize={pageSize}");
            }
            catch (Exception ex)
            {
                try
                {
                    // retry once
                    Debug.WriteLine("Retrying because:" + ex.Message);
                    System.Threading.Thread.Sleep(3000);
                    return await GetRealEstatesPerPage(page, searchString);
                }
                catch (Exception)
                {
                    throw new Exception("No response on api request");
                }            
            }
            
            while (!response.IsSuccessStatusCode && response.ReasonPhrase == "Request limit exceeded")
            {
                Debug.WriteLine("Waiting because of limit exceeded");
                System.Threading.Thread.Sleep(3000);
                response = await _httpClient.GetAsync($"?type=koop&zo=/amsterdam/{searchString}/&page={page}&pagesize={pageSize}");
            }

            var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            return jObject.SelectToken("Objects").Select(o => o.ToObject<RealEstate>()).ToArray();
        }
    }

   

    public interface IApiClient
    {
        Task<RealEstate[]> GetRealEstatesAsync(string searchString);
    }
}
