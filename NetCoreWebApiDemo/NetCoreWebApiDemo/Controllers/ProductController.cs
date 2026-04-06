using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using NetCoreWebApiDemo.Exceptions;
using NetCoreWebApiDemo.Filters;
using NetCoreWebApiDemo.Models;
using NetCoreWebApiDemo.Models.Product;
using NetCoreWebApiDemo.Services;
using System.Security.Claims;

namespace NetCoreWebApiDemo.Controllers
{
    [Route("api/[controller]")]
    // [Authorize(Policy = "SameCompanyPolicy")]
    /*
    [ServiceFilter(typeof(ApiKeyAuthorizationFilter))]
    [ServiceFilter(typeof(ResourceLogFilter))]
    [ServiceFilter(typeof(ActionLogFilter))]
    */
    //[TypeFilter(typeof(ApiKeyAuthorizationFilter))]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all product wihtout filter
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll/{companyId}")]
        // [EnableRateLimiting("fixed")]
        // [EnableRateLimiting("user-sliding")]
        // [Authorize(Policy = "Product")]
        // [Authorize(Roles = "Admin")]
        // [ServiceFilter(typeof(WrapResponseFilter))]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
        [ProducesResponseType(typeof(NoData), 400)]
        public IActionResult GetAll()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.FindFirstValue("name");
                /*
                var product = User.FindFirstValue("product");
                if (product != "true")
                {
                    return Forbid();
                }
                */
                _logger.LogInformation("GetAll fetched at: {time}", DateTime.Now);
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

        /// <summary>
        /// Return custom product
        /// </summary>
        /// <param name="id">Product unique id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotFoundException"></exception>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            /*
            if (id <= 0)
                throw new ArgumentException("Invalid id.");
            if (id == 1)
                throw new NotFoundException("");
            */
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
