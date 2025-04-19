using FluentResults;
using Kromi.Application.Data.Dto.Auth;
using Kromi.Domain.Entities;
using Kromi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kromi.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> Login(AuthLoginRequest request);
        Task DeleteUserRefreshTokens(string username, string refreshToken);
        Task<UserRefreshTokens?> GetSavedRefreshTokens(string username, string refreshToken);
        Task<Result<RefreshResponse>> RefreshToken(RefreshResponse request);
    }
}
