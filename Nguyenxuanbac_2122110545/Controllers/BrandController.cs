using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nguyenxuanbac_2122110545.Data;
using Nguyenxuanbac_2122110545.Model;
using Nguyenxuanbac_2122110545.Services;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nguyenxuanbac_2122110545.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public BrandController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetAll()
        {
            return await _context.Brands.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandResponse>> GetById(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound("Không tìm thấy thương hiệu với ID này");
            }

            return new BrandResponse
            {
                Id = brand.Id,
                Name = brand.Name,
                Slug = brand.Slug,
                ImageUrl = _fileService.GetFileUrl(brand.Image, "brands"),
                Description = brand.Description,
                SortOrder = brand.SortOrder,
                CreatedAt = brand.CreatedAt,
                Status = brand.Status
            };
        }

        [HttpPost]
        public async Task<ActionResult<Brand>> Create([FromForm] BrandCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageName = await _fileService.SaveFileAsync(request.Image, "brands");

            var newBrand = new Brand
            {
                Name = request.Name,
                Slug = request.Slug,
                Image = imageName,
                Description = request.Description,
                SortOrder = request.SortOrder,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Brands.Add(newBrand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newBrand.Id }, newBrand);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BrandUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingBrand = await _context.Brands.FindAsync(id);
            if (existingBrand == null)
            {
                return NotFound("Không tìm thấy thương hiệu để cập nhật");
            }

            if (request.Image != null)
            {
                _fileService.DeleteFile(existingBrand.Image, "brands");
                existingBrand.Image = await _fileService.SaveFileAsync(request.Image, "brands");
            }

            existingBrand.Name = request.Name;
            existingBrand.Slug = request.Slug;
            existingBrand.Description = request.Description;
            existingBrand.SortOrder = request.SortOrder;
            existingBrand.UpdatedBy = request.UpdatedBy;
            existingBrand.UpdatedAt = DateTime.UtcNow;
            existingBrand.Status = request.Status;

            _context.Entry(existingBrand).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound("Không tìm thấy thương hiệu để xóa");
            }

            _fileService.DeleteFile(brand.Image, "brands");
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class BrandCreateRequest
        {
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }

            [Required]
            public IFormFile Image { get; set; }
            public string Description { get; set; }
            public int SortOrder { get; set; }
            public int CreatedBy { get; set; }
            public int Status { get; set; }
        }

        public class BrandUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }
            public IFormFile? Image { get; set; }
            public string Description { get; set; }
            public int SortOrder { get; set; }
            public int? UpdatedBy { get; set; }
            public int Status { get; set; }
        }

        public class BrandResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
            public int SortOrder { get; set; }
            public DateTime CreatedAt { get; set; }
            public int Status { get; set; }
        }
    }
}