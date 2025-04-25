using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nguyenxuanbac_2122110545.Data;
using Nguyenxuanbac_2122110545.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nguyenxuanbac_2122110545.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            return await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }
            return order;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Post([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            order.CreateAt = DateTime.UtcNow;
            order.OrderDetails ??= new List<OrderDetail>();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order updatedOrder)
        {
            if (id != updatedOrder.Id)
            {
                return BadRequest();
            }

            var existingOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin đơn hàng
            existingOrder.CustomerName = updatedOrder.CustomerName;
            existingOrder.PhoneNumber = updatedOrder.PhoneNumber;
            existingOrder.Email = updatedOrder.Email;
            existingOrder.DeliveryDate = updatedOrder.DeliveryDate;
            existingOrder.DeliveryAddress = updatedOrder.DeliveryAddress;
            existingOrder.City = updatedOrder.City;
            existingOrder.District = updatedOrder.District;
            existingOrder.Notes = updatedOrder.Notes;
            existingOrder.TotalAmount = updatedOrder.TotalAmount;
            existingOrder.ShippingFee = updatedOrder.ShippingFee;
            existingOrder.PaymentMethod = updatedOrder.PaymentMethod;
            existingOrder.Status = updatedOrder.Status;

            // Cập nhật chi tiết đơn hàng
            if (updatedOrder.OrderDetails != null)
            {
                _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);
                existingOrder.OrderDetails = updatedOrder.OrderDetails;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("byuser/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByUserId(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            if (!orders.Any())
            {
                return NotFound("Không tìm thấy đơn hàng nào cho người dùng này");
            }
            return orders;
        }
    }
}