using System.Threading.Tasks;
using WebApiProject.DataService.IRepositories;

namespace WebApiProject.DataService.IConfiguration;

    public interface IUnitOfWork
    {
        IUsersRepository Users { get; }

        IRefreshTokensRepository RefreshTokens { get; }

        Task CompleteAsync();
        void Dispose();
    }

