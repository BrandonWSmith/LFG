using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class UserPlatform
{
    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    [Required]
    [ForeignKey("Platform")]
    public int PlatformId { get; set; }
    public Platform Platform { get; set; }
}