using System.ComponentModel.DataAnnotations;

namespace testidentity2.Data.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }

        
        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        public string NewPassword { get; set; }
    }
}
