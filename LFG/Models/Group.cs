using LFG.Enums;
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
    [Length(3, 30)]
    [RegularExpression(@"^[A-Za-z]+\\\\w{2,19}")]
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
