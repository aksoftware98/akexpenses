using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AkExpenses.Models;
using AkExpenses.Models.Interfaces;
using AkExpenses.Models.Shared;
using AkExpenses.Models.Shared.ViewModels;
using AKSoftware.WebApi.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using Splat;

namespace AkExpenses.Services
{
    public class MoneyTypeService
    {
        private ServiceClient client;
        private IConfiguration _configuration;
        private string url;

        public MoneyTypeService(IConfiguration configuration = null, ServiceClient serviceClient = null)
        {
            this._configuration = configuration ?? Locator.Current.GetService<IConfiguration>();
            this.client = serviceClient ?? Locator.Current.GetService<ServiceClient>();
            client.AccessToken = this._configuration.AccessToken;

            //Set the url
            url = this._configuration.Dictionary["SemosApiUri"].ToString();

        }

        public async Task<IEnumerable<MoneyType>> GetAll()
        {
            try
            {
                var response = await client.GetProtectedAsync<HttpCollectionResponse<MoneyType>>($"{url}/moneytypes");

                if (response != null && response.IsSuccess)
                {
                    return response.Values;
                }
                return null;
            }
            catch (Exception ex)
            {
                //TODO: Implement error logging for future analysis and fixes

                return null;
            }
        }

        public async Task<MoneyType> Get(string id)
        {
            try
            {
                var response = await client.GetProtectedAsync<HttpSingleResponse<MoneyType>>($"{url}/moneytypes/{id}");

                if (response != null && response.IsSuccess)
                {
                    return response.Value;
                }
                return null;
            }
            catch (Exception ex)
            {
                //TODO: Implement error logging for future anaylysis and fixes

                return null;
            }
        }

        public async Task<MoneyType> Create(MoneyTypeViewModel model)
        {
            try
            {
                var response = await client.PostProtectedAsync<HttpSingleResponse<MoneyType>>($"{url}/moneytypes", model);

                if (response != null && response.IsSuccess)
                {
                    return response.Value;
                }
                return null;
            }
            catch (Exception ex)
            {
                //TODO: Implement error logging for future anaylysis and fixes

                return null;
            }
        }

        public async Task<MoneyType> Edit(MoneyTypeViewModel model)
        {
            try
            {
                var response = await client.PutProtectedAsync<HttpSingleResponse<MoneyType>>($"{url}/moneytypes", model);

                if (response != null && response.IsSuccess)
                {
                    return response.Value;
                }
                return null;
            }
            catch (Exception ex)
            {
                //TODO: Implement error logging for future anaylysis and fixes

                return null;
            }
        }

        public async Task<MoneyType> Delete(string id)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _configuration.AccessToken);
                    var result = await httpClient.DeleteAsync($"{url}/moneytypes/{id}");
                    var response = JsonConvert.DeserializeObject<HttpSingleResponse<MoneyType>>(await result.Content.ReadAsStringAsync());

                    if (response != null && response.IsSuccess)
                    {
                        return response.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                //TODO: Implement error logging for future anaylysis and fixes

                return null;
            }
        }
    }
}
