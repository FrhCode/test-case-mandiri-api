namespace AuctionService.Service;

public interface IFileUploadService
{
	Task<string> UploadFileAsync(string folderName, IFormFile file);

}
