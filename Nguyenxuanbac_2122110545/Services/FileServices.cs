namespace Nguyenxuanbac_2122110545.Services

{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder);
        void DeleteFile(string fileName, string subFolder);  // Đã sửa tham số
        string GetFileUrl(string fileName, string subFolder);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", subFolder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        public void DeleteFile(string fileName, string subFolder)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", subFolder, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // Nên sử dụng logger thay vì Console.WriteLine trong ứng dụng thực tế
                Console.WriteLine($"Error deleting file: {ex.Message}");
            }
        }

        public string GetFileUrl(string fileName, string subFolder)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}/uploads/{subFolder}/{fileName}";
        }
    }
}


