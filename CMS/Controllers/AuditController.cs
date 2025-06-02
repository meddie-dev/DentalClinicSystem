using CMS.Data;
using CMS.Models;
using CMS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers
{
    public class AuditController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;



        public AuditController(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;

        }

        public async Task<IActionResult> UserListAsync()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            return View(model);
        }

        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = _roleManager.Roles.ToList();
            var roleSelections = new List<RoleSelection>();

            foreach (var role in roles)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, role.Name);
                roleSelections.Add(new RoleSelection
                {
                    RoleName = role.Name,
                    IsSelected = isInRole
                });
            }

            var model = new UserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roleSelections
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(UserRolesViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove existing roles");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add the selected role");
                    return View(model);
                }
            }

            var isDoctor = model.SelectedRole == "Doctor";
            var existingDoctor = _context.Doctors.FirstOrDefault(d => d.UserId == user.Id);

            if (isDoctor && existingDoctor == null)
            {
                _context.Doctors.Add(new Doctor
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    ContactNumber = ""
                });
                await _context.SaveChangesAsync();
            }
            else if (!isDoctor && existingDoctor != null)
            {
                _context.Doctors.Remove(existingDoctor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> Logs()
        {
            var logs = await _context.InventoryLogs.OrderByDescending(log => log.UpdatedAt).ToListAsync();

            return View(logs); 
        }


    }
}
