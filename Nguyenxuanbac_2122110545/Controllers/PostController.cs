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
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public PostController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll()
        {
            return await _context.Posts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostResponse>> GetById(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound("Không tìm thấy bài viết với ID này");
            }

            return new PostResponse
            {
                Id = post.Id,
                Title = post.Title,
                TopicId = post.TopicId,
                Content = post.Content,
                Description = post.Description,
                ThumbnailUrl = _fileService.GetFileUrl(post.Thumbnail, "posts"),
                Type = post.Type,
                ImageUrl = _fileService.GetFileUrl(post.Image, "posts"),
                Slug = post.Slug,
                SortOrder = post.SortOrder,
                Detail = post.Detail,
                CreatedAt = post.CreatedAt,
                Status = post.Status
            };
        }

        [HttpPost]
        public async Task<ActionResult<Post>> Create([FromForm] PostCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var thumbnailName = await _fileService.SaveFileAsync(request.Thumbnail, "posts");
            var imageName = await _fileService.SaveFileAsync(request.Image, "posts");

            var newPost = new Post
            {
                Title = request.Title,
                TopicId = request.TopicId,
                Content = request.Content,
                Description = request.Description,
                Thumbnail = thumbnailName,
                Type = request.Type,
                Image = imageName,
                Slug = request.Slug,
                SortOrder = request.SortOrder,
                Detail = request.Detail,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newPost.Id }, newPost);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] PostUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null)
            {
                return NotFound("Không tìm thấy bài viết để cập nhật");
            }

            if (request.Thumbnail != null)
            {
                _fileService.DeleteFile(existingPost.Thumbnail, "posts");
                existingPost.Thumbnail = await _fileService.SaveFileAsync(request.Thumbnail, "posts");
            }

            if (request.Image != null)
            {
                _fileService.DeleteFile(existingPost.Image, "posts");
                existingPost.Image = await _fileService.SaveFileAsync(request.Image, "posts");
            }

            existingPost.Title = request.Title;
            existingPost.TopicId = request.TopicId;
            existingPost.Content = request.Content;
            existingPost.Description = request.Description;
            existingPost.Type = request.Type;
            existingPost.Slug = request.Slug;
            existingPost.SortOrder = request.SortOrder;
            existingPost.Detail = request.Detail;
            existingPost.UpdatedBy = request.UpdatedBy;
            existingPost.UpdatedAt = DateTime.UtcNow;
            existingPost.Status = request.Status;

            _context.Entry(existingPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound("Không tìm thấy bài viết để xóa");
            }

            _fileService.DeleteFile(post.Thumbnail, "posts");
            _fileService.DeleteFile(post.Image, "posts");
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class PostCreateRequest
        {
            [Required]
            public string Title { get; set; }
            public int? TopicId { get; set; }
            public string Content { get; set; }
            public string Description { get; set; }

            [Required]
            public IFormFile Thumbnail { get; set; }
            public string Type { get; set; }

            [Required]
            public IFormFile Image { get; set; }
            public string Slug { get; set; }
            public int SortOrder { get; set; }
            public string Detail { get; set; }
            public int CreatedBy { get; set; }
            public int Status { get; set; }
        }

        public class PostUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Title { get; set; }
            public int? TopicId { get; set; }
            public string Content { get; set; }
            public string Description { get; set; }
            public IFormFile? Thumbnail { get; set; }
            public string Type { get; set; }
            public IFormFile? Image { get; set; }
            public string Slug { get; set; }
            public int SortOrder { get; set; }
            public string Detail { get; set; }
            public int? UpdatedBy { get; set; }
            public int Status { get; set; }
        }

        public class PostResponse
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int? TopicId { get; set; }
            public string Content { get; set; }
            public string Description { get; set; }
            public string ThumbnailUrl { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
            public string Slug { get; set; }
            public int SortOrder { get; set; }
            public string Detail { get; set; }
            public DateTime CreatedAt { get; set; }
            public int Status { get; set; }
        }
    }
}