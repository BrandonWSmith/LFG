using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LFG.Models;

public class GamePublisher
{
    [Required]
    [ForeignKey("Game")]
    public int GameId { get; set; }
    public Game Game { get; set; }

    [Required]
    [ForeignKey("Company")]
    public int CompanyId { get; set; }
    public Company Company { get; set; }
}