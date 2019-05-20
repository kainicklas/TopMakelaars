using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TopMakelaars.Models
{
    public class RealEstate
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }
        [JsonProperty("IsVerkocht")]
        public bool Sold { get; set; }
        [JsonProperty("MakelaarId")]
        public string RealEstateAgentId { get; set; }
        [JsonProperty("MakelaarNaam")]
        public string RealEstateAgentName { get; set; }

    }
}
