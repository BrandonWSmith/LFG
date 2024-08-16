using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class GameDeveloper
{
    [Required]
    [ForeignKey("Game")]
    public int GameId { get; set; }
    public Game Game { get; set; }

    [Required]
    [ForeignKey("Company")]
    public string CompanyId { get; set; }
    public Company Company { get; set; }
}