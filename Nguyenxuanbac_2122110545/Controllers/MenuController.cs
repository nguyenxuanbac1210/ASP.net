using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nguyenxuanbac_2122110545.Data;
using Nguyenxuanbac_2122110545.Model;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nguyenxuanbac_2122110545.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menu>>> GetAll()
        {
            return await _context.Menus.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Menu>> GetById(int id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
            {
                return NotFound("Không tìm thấy menu với ID này");
            }

            return menu;
        }

        [HttpPost]
        public async Task<ActionResult<Menu>> Create([FromBody] MenuCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newMenu = new Menu
            {
                Name = request.Name,
                Link = request.Link,
                Type = request.Type,
                TableId = request.TableId,
                SortOrder = request.SortOrder,
                ParentId = request.ParentId,
                Position = request.Position,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Menus.Add(newMenu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newMenu.Id }, newMenu);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MenuUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingMenu = await _context.Menus.FindAsync(id);
            if (existingMenu == null)
            {
                return NotFound("Không tìm thấy menu để cập nhật");
            }

            existingMenu.Name = request.Name;
            existingMenu.Link = request.Link;
            existingMenu.Type = request.Type;
            existingMenu.TableId = request.TableId;
            existingMenu.SortOrder = request.SortOrder;
            existingMenu.ParentId = request.ParentId;
            existingMenu.Position = request.Position;
            existingMenu.UpdatedBy = request.UpdatedBy;
            existingMenu.UpdatedAt = DateTime.UtcNow;
            existingMenu.Status = request.Status;

            _context.Entry(existingMenu).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound("Không tìm thấy menu để xóa");
            }

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class MenuCreateRequest
        {
            [Required]
            public string Name { get; set; }
            public string Link { get; set; }
            public string Type { get; set; }
            public int TableId { get; set; }
            public int SortOrder { get; set; }
            public int ParentId { get; set; }
            public string Position { get; set; }
            public int CreatedBy { get; set; }
            public int Status { get; set; }
        }

        public class MenuUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Link { get; set; }
            public string Type { get; set; }
            public int TableId { get; set; }
            public int SortOrder { get; set; }
            public int ParentId { get; set; }
            public string Position { get; set; }
            public int? UpdatedBy { get; set; }
            public int Status { get; set; }
        }
    }
}