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
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAll()
        {
            return await _context.Contacts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetById(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound("Không tìm thấy liên hệ với ID này");
            }

            return contact;
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> Create([FromBody] ContactCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newContact = new Contact
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Title = request.Title,
                Content = request.Content,
                ReplayId = request.ReplayId,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Status = request.Status
            };

            _context.Contacts.Add(newContact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newContact.Id }, newContact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContactUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingContact = await _context.Contacts.FindAsync(id);
            if (existingContact == null)
            {
                return NotFound("Không tìm thấy liên hệ để cập nhật");
            }

            existingContact.Name = request.Name;
            existingContact.Email = request.Email;
            existingContact.Phone = request.Phone;
            existingContact.Title = request.Title;
            existingContact.Content = request.Content;
            existingContact.ReplayId = request.ReplayId;
            existingContact.UpdatedBy = request.UpdatedBy;
            existingContact.UpdatedAt = DateTime.UtcNow;
            existingContact.Status = request.Status;

            _context.Entry(existingContact).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("Không tìm thấy liên hệ để xóa");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class ContactCreateRequest
        {
            [Required]
            public string Name { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public int ReplayId { get; set; }
            public int CreatedBy { get; set; }
            public int Status { get; set; }
        }

        public class ContactUpdateRequest
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public int ReplayId { get; set; }
            public int? UpdatedBy { get; set; }
            public int Status { get; set; }
        }
    }
}