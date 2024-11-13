using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Models;
using AuctionService.Repository;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repository;

public class AuctionRepository : BaseRepository<Auction>, IAuctionRepository
{
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndPoint;

    public AuctionRepository(AuctionDbContext context , IMapper mapper , IPublishEndpoint publishEndpoint) : base(context)
    {
        _mapper = mapper;
        _publishEndPoint = publishEndpoint;
    }


    public async Task<List<AuctionDto>> GetAuctionsAsync()
    {
        var auctions = await _context.Auctions.Include(a => a.Item).OrderBy(a => a.Item.Make).ToListAsync();
        return _mapper.Map<List<AuctionDto>>(auctions);
    }

    public async Task<AuctionDto> AddAuction(CreateAuctionDto auction)
    {
        var auctionEntity = _mapper.Map<Auction>(auction);
        // ToDo Add the Seller Name on creating the auction

        await _context.Auctions.AddAsync(auctionEntity);

        var newAuction = _mapper.Map<AuctionDto>(auctionEntity);
        await _publishEndPoint.Publish(_mapper.Map<AuctionCreated>(newAuction)); //sending The object To The Contracts Model

        var result = await _context.SaveChangesAsync();
        
        if(result <= 0) return null;
        return newAuction;
    }

    public async Task<Auction> Updateauction(Guid id, UpdateAuctionDto updated)
    {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
        if(auction != null){
            auction.Item.Make = updated.Make ?? auction.Item.Make;
            auction.Item.Model = updated.Model ?? auction.Item.Model;
            auction.Item.Color = updated.Color ?? auction.Item.Color;
            auction.Item.Mileage = updated.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updated.Year ?? auction.Item.Year;

            var newAuction = _mapper.Map<AuctionUpdated>(auction);

            await _publishEndPoint.Publish(newAuction);

            var result = await _context.SaveChangesAsync() > 0;
                if(result){
                    return auction;
                }
            return null;
        }
        return null;
    }

    public async Task<string> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        // ToDo check the seller Name on deleting the auction
        
        // Publishing Process
        AuctionDeleted x = new AuctionDeleted{
            Id = id
        };
        await _publishEndPoint.Publish(x);
        // end
        if(auction != null){
            _context.Auctions.Remove(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if(result){
                return "Deleted";
            }
        }
        return null;
    }
}