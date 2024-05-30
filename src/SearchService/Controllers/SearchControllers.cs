using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelper;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<Item>>> Get([FromQuery] SearchParam searchParam)
	{
		var query = DB.PagedSearch<Item, Item>();

		var isSearchTermEmpty = string.IsNullOrEmpty(searchParam.SearchTerm);
		if (!isSearchTermEmpty)
		{
			query.Match(Search.Full, searchParam.SearchTerm);
		}

		query = searchParam.OrderBy switch
		{
			"make,asc" => query.Sort(a => a.Make, Order.Ascending),
			"make,desc" => query.Sort(a => a.Make, Order.Descending),
			"model,asc" => query.Sort(a => a.Model, Order.Ascending),
			"model,desc" => query.Sort(a => a.Model, Order.Descending),
			"year,asc" => query.Sort(a => a.Year, Order.Ascending),
			"year,desc" => query.Sort(a => a.Year, Order.Descending),
			"auction,asc" => query.Sort(a => a.AuctionEnd, Order.Ascending),
			"auction,desc" => query.Sort(a => a.AuctionEnd, Order.Descending),
			"new,asc" => query.Sort(a => a.CreatedAt, Order.Ascending),
			"new,desc" => query.Sort(a => a.CreatedAt, Order.Descending),
			_ => query.Sort(a => a.AuctionEnd, Order.Ascending)
		};

		// filter by finished, ending soon
		query = searchParam.FilterBy switch
		{
			"finished" => query.Match(a => a.AuctionEnd < DateTime.UtcNow),
			"endingSoon" => query.Match(a => a.AuctionEnd > DateTime.UtcNow && a.AuctionEnd < DateTime.UtcNow.AddHours(6)),
			"running" => query.Match(a => a.AuctionEnd > DateTime.UtcNow),
			_ => query.Match(a => a.AuctionEnd > DateTime.UtcNow)
		};

		var isSellerParamEmpty = string.IsNullOrEmpty(searchParam.Seller);
		if (!isSellerParamEmpty)
		{
			query.Match(a => a.Seller == searchParam.Seller);
		}

		var isWinnerParamEmpty = string.IsNullOrEmpty(searchParam.Winner);
		if (!isWinnerParamEmpty)
		{
			query.Match(a => a.Winner == searchParam.Winner);
		}

		query.PageNumber(searchParam.PageNumber);
		query.PageSize(searchParam.PageSize);

		var data = await query.ExecuteAsync();

		return Ok(new
		{
			data.PageCount,
			data.TotalCount,
			data.Results
		});
	}
}