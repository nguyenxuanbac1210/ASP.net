using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nguyenxuanbac_2122110545.Data;
using Nguyenxuanbac_2122110545.Model;
using Nguyenxuanbac_2122110545.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nguyenxuanbac_2122110545.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public ProductController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm với ID này");
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageName = await _fileService.SaveFileAsync(request.Image, "products");

            var newProduct = new Product
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Slug = request.Slug,
                Description = request.Description,
                Content = request.Content,
                Image = imageName,
                Price = request.Price,
                SalePrice = request.SalePrice,
                Qty = request.Qty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                Status = true
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Không tìm thấy sản phẩm để cập nhật");
            }

            // Xử lý ảnh mới nếu có
            if (request.Image != null)
            {
                // Xóa ảnh cũ
                _fileService.DeleteFile(existingProduct.Image, "products");

                // Lưu ảnh mới
                existingProduct.Image = await _fileService.SaveFileAsync(request.Image, "products");
            }

            existingProduct.CategoryId = request.CategoryId;
            existingProduct.Name = request.Name;
            existingProduct.Slug = request.Slug;
            existingProduct.Description = request.Description;
            existingProduct.Content = request.Content;
            existingProduct.Price = request.Price;
            existingProduct.SalePrice = request.SalePrice;
            existingProduct.Qty = request.Qty;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            _context.Entry(existingProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm để xóa");
            }

            // Xóa ảnh
            _fileService.DeleteFile(product.Image, "products");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("bycategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetByCategoryId(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound("Không tìm thấy sản phẩm nào cho danh mục này");
            }

            return products;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        public class ProductCreateRequest
        {
            [Required]
            public int CategoryId { get; set; }
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }
            public string Description { get; set; }
            public string Content { get; set; }
            [Required]
            public IFormFile Image { get; set; }
            [Required]
            public double Price { get; set; }
            public double SalePrice { get; set; }
            public int Qty { get; set; }
        }

        public class ProductUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public int CategoryId { get; set; }
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }
            public string Description { get; set; }
            public string Content { get; set; }
            public IFormFile? Image { get; set; }
            [Required]
            public double Price { get; set; }
            public double SalePrice { get; set; }
            public int Qty { get; set; }
        }
    }
}
