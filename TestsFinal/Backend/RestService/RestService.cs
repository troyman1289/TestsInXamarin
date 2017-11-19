using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Interfaces;
using Backend.Model;
using Newtonsoft.Json;

namespace Backend.RestService
{
    public class RestService : IRestService
    {
        /// <summary>
        /// Fetching data from firebase
        /// </summary>
        /// <returns></returns>
        public async Task<IList<GlobalCalculation>> FetchGlobalCalculations()
        {
            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 12);
            var response = await client.GetAsync("https://calculation-18079.firebaseio.com/globalCalculations.json");
            if (!response.IsSuccessStatusCode) {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonToGlobalCalculations(json);
        }

        private IList<GlobalCalculation> JsonToGlobalCalculations(string json)
        {
            var globalCalculations = JsonConvert.DeserializeObject<IEnumerable<GlobalCalculation>>(json);
            return globalCalculations.ToList();
        }
    }
}
