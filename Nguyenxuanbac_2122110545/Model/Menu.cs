namespace Nguyenxuanbac_2122110545.Model
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Type { get; set; }
        public int TableId { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; } // Nullable vì cột updated_by cho phép NULL
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } // Nullable vì cột updated_at cho phép NULL
        public int Status { get; set; }
        public int SortOrder { get; set; }
        public int ParentId { get; set; }
        public string Position { get; set; }
    }
}

