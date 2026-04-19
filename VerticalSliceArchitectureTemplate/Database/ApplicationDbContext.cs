using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureTemplate.Features.Book;

namespace VerticalSliceArchitectureTemplate.Database;

public class ApplicationDbContext(DbContextOptions options)
 : DbContext(options)
{
    public DbSet<Book> Books { get; set; } = null!;
}

