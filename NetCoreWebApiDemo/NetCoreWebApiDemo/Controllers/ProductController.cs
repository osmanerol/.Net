using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreWebApiDemo.Exceptions;
using NetCoreWebApiDemo.Filters;
using NetCoreWebApiDemo.Models.Product;
using NetCoreWebApiDemo.Services;

namespace NetCoreWebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ApiKeyAuthorizationFilter))]
    [ServiceFilter(typeof(ResourceLogFilter))]
    [ServiceFilter(typeof(ActionLogFilter))]
    //[TypeFilter(typeof(ApiKeyAuthorizationFilter))]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ServiceFilter(typeof(WrapResponseFilter))]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_productService.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllFiltered")]
        public IActionResult GetAllFiltered(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 2,
            [FromQuery] string? sort = null,
            [FromQuery] string? search = null
        )
        {
            try
            {
                return Ok(_productService.GetPagedFilteredSorted(page, pageSize, sort, search));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.");
            if (id == 1)
                throw new NotFoundException("");
            var item = _productService.GetById(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public IActionResult Add(ProductSaveDto product)
        {
            try
            {
                _productService.Add(product);
                return CreatedAtAction(nameof(GetById), product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductSaveDto product)
        {
            try
            {
                _productService.Update(id, product);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _productService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
