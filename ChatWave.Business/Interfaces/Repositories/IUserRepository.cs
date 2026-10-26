namespace ChatWave.Business.Interfaces.Repositories;

public interface IUserRepository
{
    Task<int?> GetIdByPhoneAsync(string phoneNumber);

    Task AddAsync(string phoneNumber);
}
