using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    //[AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Authorize("NotAuthenticated")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Authorize("NotAuthenticated")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            //Check for validation errors
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(e => e.ErrorMessage);
                return View(registerDTO);
            }
            ApplicationUser user = new()
            {
                PersonName = registerDTO.PersonName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.Email //for login 
                 
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
            //check the status of radio button
            if (registerDTO.UserType == UserTypeOptions.Admin) {
                //create 'Admin' role 
                if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null) {
                    await _roleManager.CreateAsync(new ApplicationRole() { Name =UserTypeOptions.Admin.ToString() }); 
                }
                //add the new user inot 'Admin' role
                await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
            } else
            {
                //create 'User' role 
                if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                {
                    await _roleManager.CreateAsync(new ApplicationRole() { Name = UserTypeOptions.User.ToString() });
                }
                //Add the new user into 'User' role
                await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString()); 
            }
            if (result.Succeeded)
            {
                //Insertion successfully completed
                await _signInManager.SignInAsync(user,isPersistent:false);
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            } else
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return View(registerDTO);
            }
        }

        
        [HttpGet]
        [Authorize("NotAuthenticated")]
        public IActionResult Login()
        {
            return View();
        }
         
        [HttpPost]
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Login(LoginDTO loginDTO,string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(loginDTO);
            } 
             var result=await _signInManager.PasswordSignInAsync(loginDTO.Email!, loginDTO.Password!, isPersistent:false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                //Admin
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if(user != null)
                {
                    if(await _userManager.IsInRoleAsync(user,UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction(actionName: "Index", controllerName: "Home", routeValues: new { area = "Admin" });
                    }
                }

                if(!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            } 
            ModelState.AddModelError("Login", "Invalid email or password");
            return View(loginDTO);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
           ApplicationUser user= await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);//valid
            }else
            {
                return Json(false);//invalid
            }

        }
    }
}
