// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using OnlineClassifieds.Models.ViewModels;
using OnlineClassifieds.Services;

/// <summary>
///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>

namespace OnlineClassifieds.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly CurrentUserProvider _currentUserProvider;

        public IndexModel(
            CurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public UserVM UserVM { get; set; }

        //public class InputModel
        //{
        //    [Required]
        //    [EmailAddress]
        //    [Display(Name = "Email")]
        //    public string Email { get; set; }

        //    [Phone]
        //    [Display(Name = "PhoneNumber")]
        //    public string PhoneNumber { get; set; }

        //    [Display(Name = "FullName")]
        //    [Required(ErrorMessage = "Required FullName")]
        //    [StringLength(25, MinimumLength = 3, ErrorMessage = "FullName must be from {1} to {2}")]
        //    public string FullName { get; set; }

        //    [Display(Name = "Avatar")]
        //    public string Avatar { get; set; }
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _currentUserProvider.GetCurrentUser();
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{user.Id}'.");
            }
            UserVM = new UserVM
            {
                User = user
            };
            return Page();
        }
    }
}
