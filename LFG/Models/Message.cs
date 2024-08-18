using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class Message
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required]
  [ForeignKey("User")]
  public int UserId { get; set; }
  public User User { get; set; }

  [Required]
  [ForeignKey("Sender")]
  public int SenderId { get; set; }
  public User Sender { get; set; }

  [MaxLength(30)]
  public string? Subject { get; set; }

  [Required]
  [Length(1, 500)]
  public string Body { get; set; }

  [Required]
  public bool Unread { get; set; }

  [Required]
  [DataType(DataType.DateTime)]
  public DateTime Sent { get; set; }
}