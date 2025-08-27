using InsureYou_AI.Dtos;
using InsureYou_AI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InsureYou_AI.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRegisterDto createUserRegisterDto)
        {
            AppUser appUser = new AppUser()
            {
                Name = createUserRegisterDto.Name,
                Surname = createUserRegisterDto.Surname,
                UserName = createUserRegisterDto.Username,
                Email = createUserRegisterDto.Email,
                ImageUrl = "",
                Description = "",
                City = "",
                Education = "",
                Title = ""
            };

            await _userManager.CreateAsync(appUser, createUserRegisterDto.Password);
            return RedirectToAction("UserList", "AppUser");
        }
    }
}
