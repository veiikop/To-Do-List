using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace To_Do_List.Models
{
    public class APIDBContect : DbContext
    {
        public DbSet<TodoList> TodoLists { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        public APIDBContect(DbContextOptions<APIDBContect> options)
            : base(options) { }
    }
}
