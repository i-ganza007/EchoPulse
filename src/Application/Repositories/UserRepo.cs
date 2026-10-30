using Echo.src.Infrastructure.Persistence.SQL;
using Echo.src.Domain.Entities;
namespace Echo.src.Application.Repositories
{
    public class UserRepo
    {
        private readonly JamsDBContext _jamDBContext;
        public UserRepo(JamsDBContext jamDBContext) {
            _jamDBContext = jamDBContext;
        }

        public async Task<IQueryable<User?>> GetAllUsers() {
            return  _jamDBContext.User.Select(x=>x);
        }

        public async Task<IQueryable<User?>> GetUserBatchUsers(ICollection<Guid> _userIds)
        {
            var result =  _userIds.Select(x => _jamDBContext.User.FirstOrDefault(y => y.userId == x)).Where(x=>x != null).AsQueryable();
            return result;
        }

        public async Task<User?> GetSingleUser(Guid _userId) {
            return _jamDBContext.User.FirstOrDefault(x => x.userId == _userId);
        }
        public async Task<bool> CreateUser(User _user) {
            var result_add = await _jamDBContext.User.AddAsync(_user);
            if (result_add.State == Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                await _jamDBContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> CreateBulkUsers(ICollection<User> _users) {
            await _jamDBContext.AddRangeAsync(_users);
            return true;
        }
        public void UpdateUsers() { }


        public async Task<bool> DeleteUser(Guid _userId) {
            var userDeletion = _jamDBContext.User.FirstOrDefault(x => x.userId == _userId);
            if(userDeletion is null)
            {
                throw new Exception($"{nameof(_userId)} doesn't exist in users db");
            }
            var remove_User = _jamDBContext.User.Remove(userDeletion);
            if(remove_User.State == Microsoft.EntityFrameworkCore.EntityState.Deleted) {
                await _jamDBContext.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<bool> DeleteBatchUsers(ICollection<Guid> _userIDS) {
            var usersToBeDeleted = _userIDS.Select(x => _jamDBContext.User.FirstOrDefault(y => y.userId == x)).Where(x=>x!=null);
            _jamDBContext.User.RemoveRange(usersToBeDeleted);
            await _jamDBContext.SaveChangesAsync();
            return true;
        }
    }
}
