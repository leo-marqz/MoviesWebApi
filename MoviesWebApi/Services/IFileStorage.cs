namespace MoviesWebApi.Services
{
    public interface IFileStorage
    {
        Task<string> SaveFile(
            byte[] content, 
            string extension, 
            string container, 
            string contentType);
        Task<string> EditFile(
            byte[] content, 
            string extension, 
            string container, 
            string path, 
            string contentType);
        Task DeleteFile(
            string path, 
            string container);
    }
}
