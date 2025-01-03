using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuctionDbContext _context;

    public AuctionsController(IMapper mapper, IUnitOfWork unitOfWork, AuctionDbContext context)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
    {
        var auctions = await _unitOfWork._AuctionRepository.GetAuctionsAsync();
        return Ok(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);
        if (auction == null) return NotFound();
        return Ok(_mapper.Map<AuctionDto>(auction));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = await _unitOfWork._AuctionRepository.AddAuction(auctionDto);
        auction.Seller = User.Identity.Name;
        if(auction == null) return BadRequest("there are missing Elements");
        return CreatedAtAction(nameof(GetAuctionById) , new{id = auction.Id} , auction);
    }
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id , UpdateAuctionDto updateAuction){
        var Auction = await _context.Auctions.FindAsync(id);
        if(Auction.Seller != User.Identity.Name) Forbid();
        var auction = await _unitOfWork._AuctionRepository.Updateauction(id , updateAuction);
        if(auction == null) return BadRequest("The Auction does not exist");
        return Ok();
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id){
        var Auction = await _context.Auctions.FindAsync(id);
        if(Auction.Seller != User.Identity.Name) Forbid();
        var auction = await _unitOfWork._AuctionRepository.DeleteAuction(id);
        if(auction == null){
            return NotFound();
        }
        return Ok();
    }
}