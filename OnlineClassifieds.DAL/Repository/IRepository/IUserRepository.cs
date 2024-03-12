using OnlineClassifieds.Models;

namespace OnlineClassifieds.DAL.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> Add(User entity, string password);
        Task AddRole(User user, string role, string? deleteRole = null);
        Task<string?> GetRole(User user);
        Task<User?> Find(string id);
        void Update(User user, User oldUser);
        Task UpdateName(User user);
        Task UpdatePassword(User user, string password);
    }
}
