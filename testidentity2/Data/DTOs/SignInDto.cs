using System.ComponentModel.DataAnnotations;

namespace testidentity2.Data.DTOs
{

        public class Request
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }

        }
        public class Response
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }

        }
    
}
