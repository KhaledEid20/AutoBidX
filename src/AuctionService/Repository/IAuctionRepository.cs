using AuctionService.DTOs;
using AuctionService.Models;

namespace AuctionService.Repository;

public interface IAuctionRepository : IBaseRepository<Auction>
{
    Task<List<AuctionDto>> GetAuctionsAsync();
    Task<AuctionDto> AddAuction(CreateAuctionDto auction);
    Task<Auction> Updateauction(Guid id , UpdateAuctionDto updated);
    Task<string> DeleteAuction(Guid id);
}