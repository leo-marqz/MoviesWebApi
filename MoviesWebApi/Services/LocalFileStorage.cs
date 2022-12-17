namespace MoviesWebApi.Services
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccesor;

        public LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccesor)
        {
            this.env = env;
            this.httpContextAccesor = httpContextAccesor;
        }

        public Task DeleteFile(string path, string container)
        {
            if(path != null)
            {
                var filename = Path.GetFileName(path);
                string filePath = Path.Combine(env.WebRootPath, container, filename);
                if(File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType)
        {
            await DeleteFile(path, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var filename = $"{Guid.NewGuid()}{extension}";
            string directory = Path.Combine(env.WebRootPath, container);
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = Path.Combine(directory, filename);
            await File.WriteAllBytesAsync(path, content);
            var currentUrl = $"{httpContextAccesor.HttpContext.Request.Scheme}://{httpContextAccesor.HttpContext.Request.Host}";
            var urlDB = Path.Combine(currentUrl, container, filename).Replace("\\", "/");
            return urlDB;
        }
    }
}
