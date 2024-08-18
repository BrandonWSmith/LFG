using LFG.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class UserGroup
{
  [Required]
  [ForeignKey("User")]
  public int UserId { get; set; }
  public User User { get; set; }

  [Required]
  [ForeignKey("Group")]
  public int GroupId { get; set; }
  public Group Group { get; set; }

  [Required]
  public int Rank { get; set; }

  [Required]
  public GroupRole Role { get; set; }
}