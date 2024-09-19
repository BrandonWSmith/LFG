using LFG.Enums;
using LFG.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models
{
  public class Group
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [UniqueGroupName]
    [Length(3, 30)]
    [RegularExpression(@"^[A-Za-z]+[\w -]+")]
    public string Name { get; set; }

    [Required]
    public int Owner { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }

    [Required]
    public bool Public { get; set; }

    [Required]
    public GroupStatus Status { get; set; }

    public int? AvatarId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Created { get; set; }
  }
}
