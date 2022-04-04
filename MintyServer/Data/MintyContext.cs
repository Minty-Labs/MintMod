using Microsoft.EntityFrameworkCore;

namespace MintyServer.Data;

public class MintyContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
}