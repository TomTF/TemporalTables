using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemporalTables.DataAccess;
using TemporalTables.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCors()
    .AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString"));
    });

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


app.MapGet("/user/{id}", async (int id, [FromQuery] DateTime? timestamp, [FromServices] ApplicationDbContext context) =>
{
    var user = await context.Users
        .TemporalAsOf(timestamp ?? DateTime.UtcNow)
        .FirstOrDefaultAsync(u => u.Id == id);

    if (user is not null)
    {
        return Results.Ok(user);
    }

    return Results.NotFound();
});

app.MapPut("/user/{id}", async (int id, User user, [FromServices] ApplicationDbContext context) =>
{
    var original = await context.Users.FindAsync(id);
    if (original is null)
    {
        return Results.NotFound(id);
    }

    original.Name = user.Name;
    original.Birthday = user.Birthday;
    await context.SaveChangesAsync();
    return Results.Ok(original);
});


app.Run();