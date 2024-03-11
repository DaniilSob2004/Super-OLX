﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineClassifieds.Models
{
    public class User : IdentityUser
    {
        [DisplayName("FullName")]
        [Required(ErrorMessage = "Required FullName")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "FullName must be from {1} to {2}")]
        public string FullName { get; set; } = null!;

        [DisplayName("Avatar")]
        public string? Avatar { get; set; }
    }
}
