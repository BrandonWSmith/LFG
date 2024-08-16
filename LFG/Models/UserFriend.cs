using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class UserFriend
{
    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    [Required]
    [ForeignKey("Friend")]
    public int FriendId { get; set; }
    public User Friend { get; set; }
}