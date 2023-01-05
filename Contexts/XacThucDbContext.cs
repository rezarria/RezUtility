using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RezUtility.Models;

namespace RezUtility.Contexts;

public interface IXacThucDbContext
{
	DbSet<TokenDangXuat> TokenDangXuat { get; set; }
	int SaveChanges();
	void AddRange(IEnumerable<object> entities);
}

public class XacThucDbContext : DbContext, IXacThucDbContext
{
	public XacThucDbContext(DbContextOptions<XacThucDbContext> options) : base(options)
	{
	}

	public DbSet<TokenDangXuat> TokenDangXuat { get; set; } = null!;
	public new EntityEntry Entry<TEntity>(TEntity entity) where TEntity : class
		=> base.Entry(entity);
	public new EntityEntry Add<TEntity>(TEntity entity) where TEntity : class
		=> base.Add(entity);
	public new EntityEntry Attach<TEntity>(TEntity entity) where TEntity : class
		=> base.Attach(entity);
	public new EntityEntry Remove<TEntity>(TEntity entity) where TEntity : class
		=> base.Remove(entity);

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.Entity<TokenDangXuat>(entity =>
									  {
										  entity.Property(x => x.Token).ValueGeneratedNever();
										  entity.Property(x => x.Exp).HasConversion(new TimeSpanToTicksConverter());
									  });
	}
}