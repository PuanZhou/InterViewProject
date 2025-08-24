namespace InterViewProject.Service.IRepository
{
    public interface IAuthorizationRepository
    {
        Task<bool> CheckAccount(Credential credential);
    }
}
