using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class Thread
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required]
  [ForeignKey("User")]
  public int UserId { get; set; }
  public User User { get; set; }

  [Required]
  [ForeignKey("Group")]
  public int GroupId { get; set; }
  public Group Group { get; set; }

  [Required]
  [Length(1, 30)]
  public string Title { get; set; }

  [Required]
  [MaxLength(40000)]
  public string Body { get; set; }

  public int Rating { get; set; } = 0;

  [Required]
  public bool Pinned { get; set; }

  [Required]
  [DataType(DataType.DateTime)]
  public DateTime Created { get; set; }
}