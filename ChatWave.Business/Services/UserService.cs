using ChatWave.Business.Interfaces.Repositories;
using ChatWave.Business.Interfaces.Services;

namespace ChatWave.Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
}
