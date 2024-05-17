using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
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

app.MapGet("/hobbies", async ([FromServices] ApplicationDbContext context) =>
{
    return await context.Hobbies
        .TemporalAsOf(DateTime.Now)
        .ToListAsync();
});

app.MapPost("/hobbies", async (Hobby hobby, [FromServices] ApplicationDbContext context) =>
{
    context.Hobbies.Add(hobby);
    await context.SaveChangesAsync();
});

app.MapGet("/user/{id}", async (int id, [FromQuery] DateTime? timestamp, [FromServices] ApplicationDbContext context) =>
{
    var user = await context.Users
        .TemporalAsOf(timestamp ?? DateTime.UtcNow)
        .Include(u => u.Hobbies)
        .FirstOrDefaultAsync(u => u.Id == id);

    if (user is not null)
    {
        return Results.Ok(UserDto.From(user));
    }

    return Results.NotFound();
});


app.MapPut("/user/{id}", async (int id, UserUpdateDto dto, [FromServices] ApplicationDbContext context) =>
{
    var user = await context.Users
        .Include(u => u.Hobbies)
        .FirstOrDefaultAsync(u => u.Id == id);

    if (user is null)
    {
        return Results.NotFound(id);
    }

    user.Name = dto.Name;
    user.Birthday = dto.Birthday;

    // update hobbies
    var hobbies = await context.Hobbies.Where(h => dto.Hobbies.Contains(h.Id)).ToListAsync();
    var hobbiesToRemove = user.Hobbies.Where(h => !hobbies.Contains(h));
    foreach (var hobbyToRemove in hobbiesToRemove)
    {
        user.Hobbies.Remove(hobbyToRemove);
    }

    foreach (var hobby in hobbies)
    {
        if (user.Hobbies.Contains(hobby))
        {
            continue;
        }

        user.Hobbies.Add(hobby);
    }

    await context.SaveChangesAsync();
    return Results.Ok(UserDto.From(user));
});


app.Run();

public record UserDto(int Id, string Name, DateTime Birthday, IEnumerable<HobbyDto> Hobbies)
{
    internal static UserDto From(User user) => new UserDto(user.Id, user.Name, user.Birthday, user.Hobbies.Select(h => HobbyDto.From(h)));
}

public record UserUpdateDto(string Name, DateTime Birthday, IEnumerable<int> Hobbies);

public record HobbyDto(int Id, string Name)
{
    internal static HobbyDto From(Hobby hobby) => new HobbyDto(hobby.Id, hobby.Name);
}
