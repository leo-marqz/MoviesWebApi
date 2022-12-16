using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] validFileTypes;

        public FileTypeValidation(string[] validFileTypes)
        {
            this.validFileTypes = validFileTypes;
        }

        public FileTypeValidation(FileTypeGroup fileTypeGroup)
        {
            if(fileTypeGroup == FileTypeGroup.Image)
            {
                validFileTypes = new string[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;
            IFormFile file = value as IFormFile;
            if (file is null) return ValidationResult.Success;
            if(!validFileTypes.Contains(file.ContentType))
            {
                return new ValidationResult($"Valid file types: {string.Join(", ", validFileTypes)}");
            }
            return ValidationResult.Success;
        }
    }
}
