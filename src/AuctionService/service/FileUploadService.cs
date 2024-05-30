namespace AuctionService.Service;

public class FileUploadService : IFileUploadService
{

	public FileUploadService()
	{
	}

	public async Task<string> UploadFileAsync(string folderName, IFormFile file)
	{
		if (file == null || file.Length == 0)
		{
			return null;
		}

		// current directory
		var currentDirectory = Directory.GetCurrentDirectory();

		var folderPath = Path.Combine(currentDirectory, "Assets", folderName);
		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}


		var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
		var filePath = Path.Combine(folderPath, fileName);

		using (var stream = new FileStream(filePath, FileMode.Create))
		{
			await file.CopyToAsync(stream);

		}

		filePath = filePath.Replace(currentDirectory, string.Empty).Replace("\\", "/").TrimStart('/');
		filePath = filePath.First().ToString().ToLower() + filePath.Substring(1);
		filePath = "/" + filePath;

		return filePath;
	}
}