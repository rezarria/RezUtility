using Microsoft.EntityFrameworkCore;
using RezUtility.PhanQuyen.Models;

namespace RezUtility.PhanQuyen.Contexts;

public class XacThucDbContext : DbContext
{
	public XacThucDbContext(DbContextOptions<XacThucDbContext> options) : base(options) {}
	public DbSet<User> Users { get; set; } = null!;
	public DbSet<UserClaim> UserClaims { get; set; } = null!;
	public DbSet<Role> Roles { get; set; } = null!;
	public DbSet<RoleClaim> RoleClaims { get; set; } = null!;
	public DbSet<ToKen> ToKens { get; set; } = null!;
}