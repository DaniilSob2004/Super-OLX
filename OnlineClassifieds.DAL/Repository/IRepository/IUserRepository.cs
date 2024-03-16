using Microsoft.AspNetCore.Identity;

using OnlineClassifieds.Models;

namespace OnlineClassifieds.DAL.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> Add(User entity, string password);
        Task AddRole(User user, string role, string? deleteRole = null);
        Task<string?> GetRole(User user);
        string GetAvatar(User? user);
        Task<string?> DeleteAvatar(string id);
        Task<string?> GetPhoneNumber(User user);
        Task<IdentityResult> SetPhoneNumber(User user, string phoneNumber);
        Task<User?> Find(string id);
        void Update(User user, User oldUser);
        void Update(User user);
        Task UpdateName(User user);
        Task UpdatePassword(User user, string password);
    }
}
