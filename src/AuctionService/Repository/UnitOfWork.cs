using AuctionService.Data;
using AuctionService.Repository;
using System.Threading.Tasks;

namespace AuctionService.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuctionDbContext _context;
    public IAuctionRepository _AuctionRepository { get; }

    public UnitOfWork(AuctionDbContext context, IAuctionRepository auctionRepository)
    {
        _context = context;
        _AuctionRepository = auctionRepository;
    }

    public async Task<int> Complete()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
