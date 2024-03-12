using Microsoft.AspNetCore.Identity;

using OnlineClassifieds.DAL.Data;
using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.DAL.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly DataContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRepository(
            DataContext db,
            UserManager<IdentityUser> userManager) : base(db)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<bool> Add(User user, string password)
        {
            user.UserName = user.Email;
            var createUser = await _userManager.CreateAsync(user, password);
            return createUser.Succeeded;
        }

        public async Task AddRole(User user, string role, string? deleteRole = null)
        {
            if (deleteRole is not null)
            {
                await _userManager.RemoveFromRoleAsync(user, deleteRole);
            }
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<string?> GetRole(User user)
        {
            if (user is null) { return null; }

            string roles = "";
            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                roles += $"{role} ";
            }
            return roles.Trim();
        }

        public async Task<User?> Find(string id)
        {
            return (User?)await _userManager.FindByIdAsync(id);
        }

        public override async Task Remove(User user)
        {
            var identityUser = await FirstOrDefault(u => u.Id.Equals(user.Id));
            if (identityUser is not null)
            {
                await _userManager.DeleteAsync(identityUser);
            }
        }

        public void Update(User user, User oldUser)
        {
            user.FullName = oldUser.FullName;
            user.Email = oldUser.Email;
            _db.Update(user);
        }

        public async Task UpdateName(User user)
        {
            await _userManager.SetUserNameAsync(user, user.Email);
        }

        public async Task UpdatePassword(User user, string password)
        {
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, password);
        }
    }
}
