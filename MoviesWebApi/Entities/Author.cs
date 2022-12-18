using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Entities
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Photo { get; set; }
        //--------------------------------
        public List<MoviesAuthors> MoviesAuthors { get; set; }
    }
}
