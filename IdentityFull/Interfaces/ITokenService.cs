using FullIdentity.Data.Entities;

namespace FullIdentity.Interfaces;

public interface ITokenService
{
    string GenerateJWTToken(AppUser user);
}