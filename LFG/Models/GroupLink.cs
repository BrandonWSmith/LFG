using LFG.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LFG.Models;

public class GroupLink
{
  [Required]
  [ForeignKey("Group")]
  public int GroupId { get; set; }
  public Group Group { get; set; }

  [Required]
  public Website SiteName { get; set; }

  [Required]
  [RegularExpression(@"[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?")]
  public string Link { get; set; }
}