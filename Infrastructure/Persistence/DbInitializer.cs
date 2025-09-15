
using Microsoft.AspNetCore.Identity;

namespace Persistence;
public class DbInitializer(AppDbContext context,
    AppDbContext identityContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
    : IDbInitializer
{
    public async Task InitializeAsync()
    {
        try
        {
           
        }
        catch (Exception ex)
        {

        }
    }

    public async Task InitializeIdentityAsync()
    {
       
        
    }
}
