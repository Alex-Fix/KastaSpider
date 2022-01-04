using Core.Models.Kasta;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services.Services.Implementations
{
    public class KastaClient : IKastaClient
    {
        private HttpClient GetHttpClient()
        {
            const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36";
            const int TIMEOUT_MIN = 2;

            var client = new HttpClient();

            client.Timeout = TimeSpan.FromMinutes(TIMEOUT_MIN);
            client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);

            return client;
        }

        public async Task<IList<string>> GetSitemapUrls()
        {
            const string SITEMAP_URL = "https://kasta.ua/sitemap.xml";

            using var client = GetHttpClient();

            HttpResponseMessage response = await client.GetAsync(SITEMAP_URL);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Invalid status code");
            }

            string content = await response.Content.ReadAsStringAsync();

            var sitemapIndex = XmlSerializerHelper.Deserialize<SitemapIndexElement>(content);
            var sitemapUrls = sitemapIndex.Sitemaps
                .Select(s => s.Loc)
                .ToList();

            return sitemapUrls;
        }

        public async Task<IList<string>> GetProductSitemapUrls()
        {
            const string PRODUCT_SITEMAP_TEMPLATE = "https://kasta.ua/sitemap-products-";

            var sitemapUrls = await GetSitemapUrls();
            var productSitemapUrls = sitemapUrls
                .Where(s => s.Contains(PRODUCT_SITEMAP_TEMPLATE))
                .ToList();

            return productSitemapUrls;
        }

        public async Task<IList<string>> GetProductUrls(string productSitemapUrl)
        {
            if (string.IsNullOrEmpty(productSitemapUrl))
            {
                throw new ArgumentNullException(nameof(productSitemapUrl));
            }

            using var client = GetHttpClient();

            HttpResponseMessage response = await client.GetAsync(productSitemapUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Invalid status code");
            }

            string content = await response.Content.ReadAsStringAsync();

            var urlSet = XmlSerializerHelper.Deserialize<UrlSetElement>(content);
            var productUrls = urlSet.Urls
                .Select(u => u.Loc)
                .ToList();

            return productUrls;
        }
    }
}