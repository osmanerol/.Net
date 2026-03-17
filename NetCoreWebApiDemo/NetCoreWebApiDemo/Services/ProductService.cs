using NetCoreWebApiDemo.Models;
using NetCoreWebApiDemo.Repositories;

namespace NetCoreWebApiDemo.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repository;

        public ProductService(IGenericRepository<Product> repository)
        {
            _repository = repository;
        }

        public void Add(Product product)
        {
            _repository.Add(product);
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

        public IEnumerable<Product> GetAll()
        {
            return _repository.GetAll();
        }

        public Product? GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Update(int id, Product product)
        {
            var item = _repository.GetById(id);
            if (item == null)
                throw new Exception("Product not found.");
            item.Name = product.Name;
            item.Price = product.Price;
            item.Stock  = product.Stock;
            _repository.Update(item);
            _repository.Save();
        }
    }
}
