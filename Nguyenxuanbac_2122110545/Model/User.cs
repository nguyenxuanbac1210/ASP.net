namespace Nguyenxuanbac_2122110545.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? Update_At { get; set; } = DateTime.UtcNow;
        public DateTime? DeleteAt { get; set; }
        public string Create_By { get; set; } = "System";
        public string Update_By { get; set; } = "System";
        public bool Status { get; set; } = true;
        public string Avatar { get; set; }
    }
}

