using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models.ViewModels;
using OnlineClassifieds.Services;

namespace OnlineClassifieds.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly FilesWorkService _filesWorkService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserController(
            IUserRepository userRepository,
            FilesWorkService filesWorkService,
            SignInManager<IdentityUser> signInManager)
        {
            _userRepository = userRepository;
            _filesWorkService = filesWorkService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        // метод Edit [HttpGet] находится в Identity/Pages/Account/Manage/Index
        public async Task<IActionResult> Edit(UserVM? userVM)
        {
            if (!ModelState.IsValid || userVM is null)
            {
                return Redirect("/Identity/Account/Manage/Index");
            }

            var user = await _userRepository.FirstOrDefault(u => u.Id == userVM.User.Id);
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{userVM.User.Id}'.");
            }

            // обработка изображения пользователя
            var files = HttpContext.Request.Form.Files;
            if (files.Count != 0)
            {
                // загружаем картинку на сервер и сохраняем
                string newFilename = await _filesWorkService.DownloadFileForm(WC.ImageUserPath, files[0], user.Avatar);
                userVM.User.Avatar = newFilename;
            }
            else
            {
                userVM.User.Avatar = user.Avatar;
            }

            _userRepository.Update(user, userVM.User);

            var phoneNumber = await _userRepository.GetPhoneNumber(user);
            if (userVM.User.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userRepository.SetPhoneNumber(user, userVM.User.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return Redirect("/Identity/Account/Manage/Index");
                }
            }

            // если email(UserName) изменен, то устанавливаем новый (email совпадает с именем)
            if (!user.UserName.Equals(user.Email))
            {
                await _userRepository.UpdateName(user);
            }

            await _userRepository.Save();
            await _signInManager.RefreshSignInAsync(user);
            return Redirect("/Identity/Account/Manage/Index");
        }
    
        public async Task<ActionResult> DeleteAvatar(string id)
        {
            if (id is not null)
            {
                string? delFilename = await _userRepository.DeleteAvatar(id);
                if (delFilename is not null)
                {
                    _filesWorkService.Delete(WC.ImageUserPath, delFilename);
                }
            }
            return Redirect("/Identity/Account/Manage/Index");
        }
    }
}
