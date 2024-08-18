using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class GroupGame
{
  [Required]
  [ForeignKey("Group")]
  public int GroupId { get; set; }
  public Group Group { get; set; }

  [Required]
  [ForeignKey("Game")]
  public int GameId { get; set; }
  public Game Game { get; set; }
}