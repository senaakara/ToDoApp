using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserByEmailAsync(string email);
}
