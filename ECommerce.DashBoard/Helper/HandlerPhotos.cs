using System.Linq;

namespace ECommerce.DashBoard.Helper
{
    public static class HandlerPhotos
    {
        private static IWebHostEnvironment _environment;
        private static string _imagePath;


        public static void Initialize(IWebHostEnvironment webHostEnvironment)
        {
            _environment = webHostEnvironment;
            _imagePath = $"{_environment.WebRootPath}";
        }

        public static string UploadPhoto(IFormFile file, string folderName)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return "Invalid image format.";
            }
            string uploadDir = Path.Combine(_imagePath,"images",folderName);
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            
            var path = Path.Combine(_imagePath,  "images", folderName, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }
        public static void DeletePhoto(string folderName, string fileName)
        {
            var path = Path.Combine(_imagePath, "Images", folderName, fileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
