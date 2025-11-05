using Microsoft.EntityFrameworkCore;
using To_Do_List.Mappings;
using To_Do_List.Models;
using To_Do_List.Repositories;
using To_Do_List.Services;
using To_Do_List.Middleware;
using AutoMapper;


namespace To_Do_List
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<APIDBContect>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnection")));
            
            // Репозитории
            builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();
            builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();

            // Сервисы
            builder.Services.AddScoped<ITodoListService, TodoListService>();
            builder.Services.AddScoped<ITodoItemService, TodoItemService>();

            ILoggerFactory factory = new LoggerFactory();
            builder.Services.AddSingleton<IMapper>(_ =>
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>(); 
                }, factory);
                return configuration.CreateMapper();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Middleware для ошибок
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
