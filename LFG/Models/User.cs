using LFG.Validation;
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
    [UniqueUsername]
    [Length(3, 20)]
    [RegularExpression(@"^[A-Za-z]+\w+")]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [UniqueEmail]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Length(1, 20)]
    [RegularExpression(@"[A-Za-z]+")]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Show First Name")]
    public bool FirstNamePublic { get; set; } = true;

    [Length(1, 20)]
    [RegularExpression(@"[A-Za-z]+")]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Show Last Name")]
    public bool LastNamePublic { get; set; } = true;

    [MaxLength(250)]
    public string? Bio { get; set; }

    [Display(Name = "Show Bio")]
    public bool BioPublic { get; set; } = true;

    public int? AvatarId { get; set; }

    public int Score { get; set; } = 0;

    [Required]
    [DataType(DataType.Date)]
    public DateTime Created { get; set; }

    [Required]
    [Display(Name = "Remember Me")]
    public bool Persistent { get; set; } = false;

    [Display(Name = "Show Platforms")]
    public bool PlatformsPublic { get; set; } = true;

    [Display(Name = "Show Groups")]
    public bool GroupsPublic { get; set; } = true;
  }
}
