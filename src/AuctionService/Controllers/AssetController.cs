using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/assets")]
public class AssetController : ControllerBase
{
	public AssetController() { }

	// [Authorize]
	[HttpGet("{**path}")]
	public IActionResult GetAsset(string path)
	{
		var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", path);
		if (!System.IO.File.Exists(filePath))
		{
			return NotFound();
		}

		var file = System.IO.File.OpenRead(filePath);

		var provider = new FileExtensionContentTypeProvider();
		if (!provider.TryGetContentType(filePath, out var contentType))
		{
			contentType = "application/octet-stream";
		}

		return File(file, contentType);
	}
}