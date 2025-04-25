using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nguyenxuanbac_2122110545.Data;
using Nguyenxuanbac_2122110545.Model;
using Nguyenxuanbac_2122110545.Services;
using System.ComponentModel.DataAnnotations;

namespace Nguyenxuanbac_2122110545.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public BannerController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> GetAll()
        {
            return await _context.Banners.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BannerResponse>> GetById(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
            {
                return NotFound("Không tìm thấy banner với ID này");
            }

            return new BannerResponse
            {
                Id = banner.Id,
                Name = banner.Name,
                Link = banner.Link,
                ImageUrl = _fileService.GetFileUrl(banner.Image, "banners"),
                Description = banner.Description,
                Position = banner.Position,
                SortOrder = banner.SortOrder,
                CreatedAt = banner.CreatedAt,
                Status = banner.Status
            };
        }

        [HttpPost]
        public async Task<ActionResult<Banner>> Create([FromForm] BannerCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageName = await _fileService.SaveFileAsync(request.Image, "banners");

            var newBanner = new Banner
            {
                Name = request.Name,
                Link = request.Link,
                Image = imageName,
                Description = request.Description,
                Position = request.Position,
                SortOrder = request.SortOrder,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Banners.Add(newBanner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newBanner.Id }, newBanner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BannerUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingBanner = await _context.Banners.FindAsync(id);
            if (existingBanner == null)
            {
                return NotFound("Không tìm thấy banner để cập nhật");
            }

            if (request.Image != null)
            {
                _fileService.DeleteFile(existingBanner.Image, "banners");
                existingBanner.Image = await _fileService.SaveFileAsync(request.Image, "banners");
            }

            existingBanner.Name = request.Name;
            existingBanner.Link = request.Link;
            existingBanner.Description = request.Description;
            existingBanner.Position = request.Position;
            existingBanner.SortOrder = request.SortOrder;
            existingBanner.UpdatedBy = request.UpdatedBy;
            existingBanner.UpdatedAt = DateTime.UtcNow;
            existingBanner.Status = request.Status;

            _context.Entry(existingBanner).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound("Không tìm thấy banner để xóa");
            }

            _fileService.DeleteFile(banner.Image, "banners");
            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class BannerCreateRequest
    {
        [Required]
        public string Name { get; set; }
        public string Link { get; set; }

        [Required]
        public IFormFile Image { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public int SortOrder { get; set; }
        public int CreatedBy { get; set; }
        public int Status { get; set; }
    }

    public class BannerUpdateRequest
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Link { get; set; }
        public IFormFile? Image { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public int SortOrder { get; set; }
        public int? UpdatedBy { get; set; }
        public int Status { get; set; }
    }

    public class BannerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Position { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
    }
}