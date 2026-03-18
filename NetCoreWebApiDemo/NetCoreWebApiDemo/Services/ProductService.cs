using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetCoreWebApiDemo.Models;
using NetCoreWebApiDemo.Models.Product;
using NetCoreWebApiDemo.Repositories;
using System.Globalization;

namespace NetCoreWebApiDemo.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly IMapper _mappper;

        public ProductService(IGenericRepository<Product> repository, IMapper mapper)
        {
            _repository = repository;
            _mappper = mapper;
        }

        public void Add(ProductSaveDto product)
        {
            var productEntity = _mappper.Map<Product>(product);
            _repository.Add(productEntity);
            _repository.Save();
        }

        public void Delete(int id)
        {
            var item = _repository.GetById(id);
            if (item == null)
                throw new Exception("Product not found.");
            _repository.Delete(item);
            _repository.Save();
        }

        public IEnumerable<ProductDto> GetAll()
        {
            IEnumerable<Product> products = _repository.GetAll();
            var productList = _mappper.Map<List<ProductDto>>(products);
            return productList;
        }

        public Result<ProductDto> GetPagedFilteredSorted(int page, int pageSize, string? sort, string? search)
        {
            var query = _repository.GetAllQueryable();
            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }
            query = ApplySorting(query, sort);
            var totalCount = query.Count();
            var data = query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToList();
            var dataList = _mappper.Map<List<ProductDto>>(data);
            return new Result<ProductDto>(dataList, totalCount, page, pageSize);
        }

        public ProductDto? GetById(int id)
        {
            var entity = _repository.GetById(id);
            var product = _mappper.Map<ProductDto>(entity);
            return product;
        }

        public void Update(int id, ProductSaveDto product)
        {
            var item = _repository.GetById(id);
            if (item == null)
                throw new Exception("Product not found.");
            var productEntity = _mappper.Map<Product>(product);
            _repository.Update(item);
            _repository.Save();
        }

        private IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sort)
        {
            if(string.IsNullOrEmpty(sort))
            {
                return query.OrderBy(p => p.Id);
            }
            TextInfo textInfo = new CultureInfo("en-US").TextInfo;
            sort = textInfo.ToTitleCase(sort);
            bool descending = sort.StartsWith("-");
            string property = descending ? sort.Substring(1) : sort;
            return descending 
                ? query.OrderByDescending(p => EF.Property<object>(p, property))
                : query.OrderBy(p => EF.Property<object>(p, property));
        }
    }
}
