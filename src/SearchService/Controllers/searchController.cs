using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Models;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class searchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> search([FromQuery]SearchParams search)
        {
            var query = DB.PagedSearch<Item , Item>();

            query.Sort(a => a.Ascending(x => x.Make));

            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                query.Match(Search.Full, search.SearchTerm).SortByTextScore();
            }
            if (!string.IsNullOrEmpty(search.OrderBy))
            {
                query = search.OrderBy switch
                {
                    "make" => query.Sort(x => x.Ascending(a => a.Make)),
                    "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
                    _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
                };
            }
            if (!string.IsNullOrEmpty(search.FilterBy))
            {
                query = search.FilterBy switch 
                {
                    "finished" => query.Match(x=> x.AuctionEnd > DateTime.UtcNow),
                    "endingSoon" => query.Match(x=> x.AuctionEnd < DateTime.UtcNow.AddDays(6) && x.AuctionEnd > DateTime.UtcNow),
                    _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
                };
            }

            if(!string.IsNullOrEmpty(search.Winner)){
                query.Match(x => x.Winner == search.Winner);
            }

            if(!string.IsNullOrEmpty(search.Seller)){
                query.Match(x => x.Seller == search.Seller);
            }

            query.PageNumber(search.PageNumber);
            query.PageSize(search.PageSize);

            var result = await query.ExecuteAsync();
            return Ok(new{                
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}
