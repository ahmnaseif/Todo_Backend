using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Todo_Backend.Models;

namespace Todo_Backend.Data
{
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<TaskItem> Tasks { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<TaskItem>().ToTable("task");
            }
        }
}
