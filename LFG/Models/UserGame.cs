using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class UserGame
{
  [Required]
  [ForeignKey("User")]
  public int UserId { get; set; }
  public User User { get; set; }

  [Required]
  [ForeignKey("Game")]
  public int GameId { get; set; }
  public Game Game { get; set; }
}