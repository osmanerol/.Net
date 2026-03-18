using NetCoreWebApiDemo.Models;
using NetCoreWebApiDemo.Models.Product;

namespace NetCoreWebApiDemo.Services
{
    public interface IProductService
    {
        IEnumerable<ProductDto> GetAll();
        Result<ProductDto> GetPagedFilteredSorted(int page, int pageSize, string? sort, string? search);
        ProductDto? GetById(int id);
        void Add(ProductSaveDto product);
        void Update(int id, ProductSaveDto product);
        void Delete(int id);
    }
}
