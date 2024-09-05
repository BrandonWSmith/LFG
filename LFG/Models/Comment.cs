using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class Comment
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required]
  [ForeignKey("User")]
  public int UserId { get; set; }
  public User User { get; set; }

  [Required]
  [ForeignKey("Thread")]
  public int ThreadId { get; set; }
  public Thread Thread { get; set; }

  [Required]
  [Length(1, 500)]
  [Display(Name = "Comment")]
  public string Body { get; set; }

  public int Rating { get; set; } = 0;

  [Required]
  [DataType(DataType.DateTime)]
  public DateTime Created { get; set; }

  public List<int> HasUpVoted { get; set; } = [];

  public List<int> HasDownVoted { get; set; } = [];
}