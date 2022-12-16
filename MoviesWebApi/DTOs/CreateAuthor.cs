using MoviesWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class CreateAuthor
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        //leomarqz
        [FileSizeValidation(maximunSizeMB: 4)]
        [FileTypeValidation(fileTypeGroup: FileTypeGroup.Image)]
        public IFormFile Photo { get; set; }
    }
}
