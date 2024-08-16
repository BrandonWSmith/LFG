using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Length(3, 20)]
        [RegularExpression(@"^[A-Za-z]+\\\\w{2,19}")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Length(1, 20)]
        [RegularExpression(@"[A-Za-z]")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Length(1, 20)]
        [RegularExpression(@"[A-Za-z]")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [MaxLength(250)]
        public string? Bio { get; set; }

        public int? AvatarId { get; set; }

        public int? Score { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
    }
}
