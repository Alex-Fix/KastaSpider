using Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IProductService
    {
        Task InsertRange(IEnumerable<Product> products);
    }
}