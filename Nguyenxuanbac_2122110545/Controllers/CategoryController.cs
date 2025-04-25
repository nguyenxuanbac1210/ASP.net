using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    [Authorize]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public CategoryController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Không tìm thấy category với ID này");
            }

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Parent_Id = category.Parent_Id,
                Sort_Order = category.Sort_Order,
                ImageUrl = _fileService.GetFileUrl(category.Image, "categories"),
                Description = category.Description,
                CreateAt = category.CreateAt,
                Status = category.Status
            };
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromForm] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageName = await _fileService.SaveFileAsync(request.Image, "categories");

            var newCategory = new Category
            {
                Name = request.Name,
                Slug = request.Slug,
                Parent_Id = request.Parent_Id,
                Sort_Order = request.Sort_Order,
                Image = imageName,
                Description = request.Description,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                CreateBy = "System",
                UpdateBy = "System",
                Status = true
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CategoryUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound("Không tìm thấy category để cập nhật");
            }

            // Xử lý ảnh mới nếu có
            if (request.Image != null)
            {
                // Xóa ảnh cũ
                _fileService.DeleteFile(existingCategory.Image, "categories");

                // Lưu ảnh mới
                existingCategory.Image = await _fileService.SaveFileAsync(request.Image, "categories");
            }

            existingCategory.Name = request.Name;
            existingCategory.Slug = request.Slug;
            existingCategory.Parent_Id = request.Parent_Id;
            existingCategory.Sort_Order = request.Sort_Order;
            existingCategory.Description = request.Description;
            existingCategory.UpdateAt = DateTime.UtcNow;

            _context.Entry(existingCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Không tìm thấy category để xóa");
            }

            // Xóa ảnh
            _fileService.DeleteFile(category.Image, "categories");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
        public class CategoryCreateRequest
        {
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }
            public int Parent_Id { get; set; }
            public int Sort_Order { get; set; }

            [Required(ErrorMessage = "Image is required")]
            [DataType(DataType.Upload)]
            //[MaxFileSize(5 * 1024 * 1024)] // 5MB
            //[AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
            public IFormFile Image { get; set; }
            public string Description { get; set; }
        }

        public class CategoryUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Slug { get; set; }
            public int Parent_Id { get; set; }
            public int Sort_Order { get; set; }
            public IFormFile? Image { get; set; }
            public string Description { get; set; }
        }
        public class CategoryResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
            public int Parent_Id { get; set; }
            public int Sort_Order { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
            public DateTime CreateAt { get; set; }
            public bool Status { get; set; }
        }
    }
}