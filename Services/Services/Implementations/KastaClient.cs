using Core.Models.Kasta;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json;
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
        #region Methods
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

            using HttpClient client = GetHttpClient();

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

            IList<string> sitemapUrls = await GetSitemapUrls();
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

            using HttpClient client = GetHttpClient();

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

        public async Task<ProductModel> GetProduct(string productUrl)
        {
            if (string.IsNullOrEmpty(productUrl))
            {
                throw new ArgumentNullException(nameof(productUrl));
            }

            using HttpClient client = GetHttpClient();

            HttpResponseMessage response = await client.GetAsync(productUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            string name = doc.QuerySelector(".p__name")?.InnerText;
            string description = doc.QuerySelector("#pdDesc .rich-text")?.InnerText;

            var categories = doc.QuerySelectorAll(".breadcrumbs_link span")
                ?.Select(n => n.InnerText)
                ?.ToList();

            string imageUrlsSerialized = doc.QuerySelector(".p__gallery")
                ?.GetAttributeValue("data-images", "[]")
                ?.Replace("&quot;", "\"");
            var imageUrls = JsonConvert.DeserializeObject<IEnumerable<string>>(imageUrlsSerialized);

            var sizes = doc.QuerySelectorAll(".p__size")
                ?.Select(n => n.InnerText)
                ?.ToList();

            var features = doc.QuerySelectorAll("#pdProps tr")
                ?.Select(n =>
                {
                    var tds = n.QuerySelectorAll("td");

                    return new FeatureModel
                    {
                        Key = tds.First().QuerySelector("span").InnerText,
                        Value = tds.Last().InnerText
                    };
                })
                ?.ToList();

            double price = default(double);
            string currency = default(string);
            var splitedPrice = doc.QuerySelector(".p__price")?.InnerText?.Split(" ");
            if(splitedPrice?.Length == 2)
            {
                price = double.TryParse(splitedPrice?.First(), out double p) ? p : default(double);
                currency = splitedPrice?.Last();
            }

            string oldPriceStr = doc.QuerySelector(".p__old-price")?.InnerText?.Split(" ")?.First();
            double oldPrice = double.TryParse(oldPriceStr, out double op) ? op : default(double);

            double discount = Math.Round(1.0 - (oldPrice == default(double) ? 1.0 : price / oldPrice), 2);

            return new ProductModel
            {
                Url = productUrl,
                Name = name,
                Description = description,
                Categories = categories.Any() ? categories : null,
                ImageUrls = imageUrls.Any() ? imageUrls : null,
                Sizes = sizes.Any() ? sizes : null,
                Price = price,
                Currency = currency,
                OldPrice = oldPrice,
                Discount = discount,
                Features = features.Any() ? features : null
            };
        }
        #endregion
    }
}