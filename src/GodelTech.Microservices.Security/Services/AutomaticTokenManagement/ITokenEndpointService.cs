using System.Threading.Tasks;
using IdentityModel.Client;

namespace GodelTech.Microservices.Security.Services.AutomaticTokenManagement
{
    public interface ITokenEndpointService
    {
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
        Task<TokenRevocationResponse> RevokeTokenAsync(string refreshToken);
    }
}