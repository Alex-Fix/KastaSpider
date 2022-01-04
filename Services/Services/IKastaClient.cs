using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IKastaClient
    {
        Task<IList<string>> GetSitemapUrls();
        Task<IList<string>> GetProductSitemapUrls();
        Task<IList<string>> GetProductUrls(string productSitemapUrl);
    }
}