using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MoviesWebApi.Services
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly string connectionString;
        public AzureFileStorage(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("AzureBlobStorage");
        }

        public async Task DeleteFile(string path, string container)
        {
            if (string.IsNullOrEmpty(path)) return;
            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            var filename = Path.GetFileName(path);
            var blob = client.GetBlobClient(filename);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> EditFile(
            byte[] content, 
            string extension, 
            string container, 
            string path, 
            string contentType)
        {
            await DeleteFile(path, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(
            byte[] content, 
            string extension, 
            string container, 
            string contentType)
        {
            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            client.SetAccessPolicy(PublicAccessType.Blob);
            var filename = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(filename);
            var blobUploadOptions = new BlobUploadOptions();
            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = contentType;
            blobUploadOptions.HttpHeaders = blobHttpHeader;
            await blob.UploadAsync(new BinaryData(content), blobUploadOptions);
            return blob.Uri.ToString();
        }
    }
}
