using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private UserManager<User> userManager;
    private RoleManager<IdentityRole> roleManager;

    public UserController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    [HttpPut]
    public async Task<string> AddUserRole([FromQuery] string userId, [FromQuery] string roleName)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return "Failed to find user";
        }

        //await roleManager.CreateAsync(new IdentityRole(roleName));

        await userManager.AddToRoleAsync(user, roleName);
        return "Added role " + roleName + " to user " + user.UserName;
    }
}
