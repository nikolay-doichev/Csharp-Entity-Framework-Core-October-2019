namespace PetStore.Services
{
    public interface IUserService
    {
        void Register(string name, string email);
        bool Exist(int userId);
    }
}
