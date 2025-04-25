using System.ComponentModel.DataAnnotations;

namespace Nguyenxuanbac_2122110545.Model
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}


