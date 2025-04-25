namespace Nguyenxuanbac_2122110545.Model
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int ReplayId { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; } // Nullable vì cột updated_by cho phép NULL
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } // Nullable vì cột updated_at cho phép NULL
        public int Status { get; set; }
    }
}

