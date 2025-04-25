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
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public UserController(AppDbContext context, IFileService fileService)
        {
            Console.WriteLine("FileService is null: " + (fileService == null));
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await _context.Users.Where(u => u.Status).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Status);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post([FromForm] UserCreateRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and Password are required");
            }

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return Conflict("Username already exists");
            }

            string avatarName = "default-avatar.png";
            if (request.Avatar != null)
            {
                avatarName = await _fileService.SaveFileAsync(request.Avatar, "avatars");
            }

            var user = new User
            {
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Role = request.Role,
                Avatar = avatarName,
                Status = true,
                CreatedAt = DateTime.UtcNow,
                Update_At = DateTime.UtcNow,
                Create_By = "System"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] UserUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Xử lý avatar mới nếu có
            if (request.Avatar != null)
            {
                // Xóa avatar cũ nếu không phải là default
                if (existingUser.Avatar != "default-avatar.png")
                {
                    _fileService.DeleteFile(existingUser.Avatar, "avatars");
                }

                // Lưu avatar mới
                existingUser.Avatar = await _fileService.SaveFileAsync(request.Avatar, "avatars");
            }

            existingUser.Username = request.Username;
            existingUser.FullName = request.FullName;
            existingUser.Email = request.Email;
            existingUser.Phone = request.Phone;
            existingUser.Address = request.Address;
            existingUser.Role = request.Role;
            existingUser.Update_At = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Xóa avatar nếu không phải là default
            if (user.Avatar != "default-avatar.png")
            {
                _fileService.DeleteFile(user.Avatar, "avatars");
            }

            user.Status = false;
            user.DeleteAt = DateTime.UtcNow;
            user.Update_At = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        public class UserCreateRequest
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Role { get; set; }
            public IFormFile? Avatar { get; set; }
        }

        public class UserUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Username { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Role { get; set; }
            public IFormFile? Avatar { get; set; }
        }
    }
}
