namespace OnlineClassifieds.Services
{
    public class FilesWorkService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilesWorkService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void Delete(string localPath, string filename)
        {
            string uploadDir = _webHostEnvironment.WebRootPath + localPath;  // директория, где хранятся изображения
            if (File.Exists(Path.Combine(uploadDir, filename)))
            {
                File.Delete(Path.Combine(uploadDir, filename));
            }
        }

        // загружает картинку переданную из формы на сервер
        public async Task<string> DownloadFileForm(string localPath, IFormFile newFile, string? oldFilename = null)
        {
            string uploadDir = _webHostEnvironment.WebRootPath + localPath;  // директория, где хранятся изображения
            if (oldFilename is not null)
            {
                this.Delete(localPath, oldFilename);  // удаляем старую картинку
            }
            string filename = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(newFile.FileName);
            using (var fileStream = new FileStream(Path.Combine(uploadDir, filename + extension), FileMode.Create))
            {
                await newFile.CopyToAsync(fileStream);
            }
            return filename + extension;  // возвращается новое название скаченной картинки
        }
    }
}
