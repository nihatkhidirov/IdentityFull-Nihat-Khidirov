using Microsoft.AspNetCore.Identity;

namespace FullIdentity.Data.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; }
}
