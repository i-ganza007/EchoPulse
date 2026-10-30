using Echo.src.Application.Repositories;
using Echo.src.Domain.Entities;
using Echo.src.Infrastructure.Persistence.SQL;
namespace Echo.src.Application.Services
{
    public class UserService
    {
        private readonly UserRepo _userRepo;
        public UserService(UserRepo userRepo) {
            _userRepo = userRepo;
        }

        public async Task<IQueryable<User?>> GetAllUsers()
        {
            return await _userRepo.GetAllUsers();
        }

        public async Task<IQueryable<User?>> GetUserBatchUsers(ICollection<Guid> _userIds)
        {
            return await _userRepo.GetUserBatchUsers(_userIds);
        }

        public async Task<User?> GetSingleUser(Guid _userId)
        {
            return await _userRepo.GetSingleUser(_userId);
        }
        public async Task<bool> CreateUser(User _user)
        {
            return await _userRepo.CreateUser(_user);
        }
        public async Task<bool> CreateBulkUsers(ICollection<User> _users)
        {
            return await _userRepo.CreateBulkUsers(_users);
        }
        public void UpdateUsers() { }


        public async Task<bool> DeleteUser(Guid _userId)
        {
            return await _userRepo.DeleteUser(_userId);
        }


        public async Task<bool> DeleteBatchUsers(ICollection<Guid> _userIDS)
        {
            return await _userRepo.DeleteBatchUsers(_userIDS);        }

    }
}
