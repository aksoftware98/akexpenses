using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
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
    public class CategoryService
    {
        private ServiceClient client;
        private IConfiguration _configuration;
        private string url;

        public CategoryService(IConfiguration configuration = null, ServiceClient serviceClient = null)
        {
            this._configuration = configuration ?? Locator.Current.GetService<IConfiguration>();
            this.client = serviceClient ?? Locator.Current.GetService<ServiceClient>();
            client.AccessToken = this._configuration.AccessToken;

            //Set the url
            url = this._configuration.Dictionary["SemosApiUri"].ToString();

        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            try
            {

                var response = await client.GetProtectedAsync<HttpCollectionResponse<Category>>($"{url}/categories");

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

        public async Task<Category> Get(string id)
        {
            try
            {
                var response = await client.GetProtectedAsync<HttpSingleResponse<Category>>($"{url}/categories/{id}");

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

        public async Task<Category> Create(CategoryViewModel model)
        {
            try
            {
                var response = await client.PostProtectedAsync<HttpSingleResponse<Category>>($"{url}/categories", model);

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

        public async Task<Category> UploadIcon(string id, string fileName)
        {
            try
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var file = new FormFile(stream, 0, stream.Length, "file", fileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/" + Path.GetExtension(fileName)
                    };

                    var response = await client.PostProtectedAsync<HttpSingleResponse<Category>>($"{url}/categories/{id}", file);

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

        public async Task<Category> Edit(CategoryViewModel model)
        {
            try
            {
                var response = await client.PutProtectedAsync<HttpSingleResponse<Category>>($"{url}/categories", model);

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

        public async Task<Category> Delete(string id)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _configuration.AccessToken);
                    var result = await httpClient.DeleteAsync($"{url}/categories/{id}");
                    var response = JsonConvert.DeserializeObject<HttpSingleResponse<Category>>(await result.Content.ReadAsStringAsync());

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
