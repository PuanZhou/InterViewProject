namespace InterViewProject.Serverice
{
    public class AuthorizationRepositoryService : IAuthorizationRepository
    {
        private readonly CoffeeContext _dbContext;
        private readonly ILogger<AuthorizationRepositoryService> _logger;
        public AuthorizationRepositoryService(CoffeeContext dbContext, ILogger<AuthorizationRepositoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> CheckAccount(Credential credential)
        {
            var target = await _dbContext.Admins.FirstOrDefaultAsync(x => x.Email == credential.Email);
            if (target is not null) 
            { 
                if (target.Password == credential.Password) 
                { 
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
