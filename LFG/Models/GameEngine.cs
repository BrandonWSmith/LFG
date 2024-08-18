using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class GameEngine
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required]
  public string Name { get; set; }

  [Required]
  [ForeignKey("Company")]
  public int CompanyId { get; set; }
  public Company Company { get; set; }

  public int? LogoId { get; set; }
}