using MassTransit;

namespace AuctionService.Data
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
        {
        }
        public DbSet<Auction> Auctions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Auction>()
                .HasOne(a => a.Item)
                .WithOne(i => i.Auction)
                .HasForeignKey<Item>(i => i.AuctionId);
                
            // Adds a table to store the state of processed messages, preventing duplicate processing
            modelBuilder.AddInboxStateEntity();
            // Adds a table to store outgoing messages before they are sent
            modelBuilder.AddOutboxMessageEntity();
            // Adds a table to track the state of the outbox, managing message delivery
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
