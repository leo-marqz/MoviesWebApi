using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Validations
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int maximunSizeMB;

        public FileSizeValidation(int maximunSizeMB)
        {
            this.maximunSizeMB = maximunSizeMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;
            IFormFile file = value as IFormFile;
            if (file is null) return ValidationResult.Success;
            if(file.Length > maximunSizeMB * 1024 * 1024)
            {
                return new ValidationResult($"the file size cannot be larger than {maximunSizeMB} megabytes");
            }
            return ValidationResult.Success;
        }
    }
}
