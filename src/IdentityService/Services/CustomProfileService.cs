using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class CustomProfileServices : IProfileService
{
	private readonly UserManager<ApplicationUser> _userManager;

	public CustomProfileServices(UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task GetProfileDataAsync(ProfileDataRequestContext context)
	{
		var sub = context.Subject;
		var user = await _userManager.GetUserAsync(sub);
		var principal = await _userManager.GetClaimsAsync(user);

		var claims = new List<Claim>
		{
			new Claim("userName", user.UserName),
		};

		context.IssuedClaims.AddRange(principal);
		context.IssuedClaims.AddRange(claims);
	}

	public async Task IsActiveAsync(IsActiveContext context)
	{
		await Task.CompletedTask;
	}
}