namespace Nguyenxuanbac_2122110545.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Parent_Id { get; set; }
        public int Sort_Order { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeleteAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; } = "System";
        public string UpdateBy { get; set; } = "System";
        public bool Status { get; set; } = true;
    }
}

