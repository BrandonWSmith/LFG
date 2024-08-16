using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class GamePlatform
{
    [Required]
    [ForeignKey("Game")]
    public int GameId { get; set; }
    public Game Game { get; set; }

    [Required]
    [ForeignKey("Platform")]
    public int PlatformId { get; set; }
    public Platform Platform { get; set; }
}