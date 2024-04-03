using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Practice21.MinimalAPI.Data;
using Practice21.MinimalAPI.Models;
using Microsoft.AspNetCore.Authorization;
namespace Practice21.MinimalAPI;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Users").WithTags(nameof(User));

        group.MapGet("/", [Authorize(Roles = "admin")] async (UserContext db) =>
        {
            return await db.Users.ToListAsync();
        })
        .WithName("GetAllUsers")
        .WithOpenApi();

        group.MapGet("/{id}", [Authorize(Roles = "admin")] async Task<Results<Ok<User>, NotFound>> (int id, UserContext db) =>
        {
            return await db.Users.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is User model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetUserById")
        .WithOpenApi();

        group.MapPut("/{id}", [Authorize(Roles = "admin")] async Task<Results<Ok, NotFound>> (int id, User user, UserContext db) =>
        {
            var affected = await db.Users
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, user.Id)
                    .SetProperty(m => m.Login, user.Login)
                    .SetProperty(m => m.Password, user.Password)
                    .SetProperty(m => m.RoleId, user.RoleId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateUser")
        .WithOpenApi();

        group.MapPost("/", [Authorize(Roles = "admin")] async (User user, UserContext db) =>
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/User/{user.Id}",user);
        })
        .WithName("CreateUser")
        .WithOpenApi();

        group.MapDelete("/{id}", [Authorize(Roles = "admin")] async Task<Results<Ok, NotFound>> (int id, UserContext db) =>
        {
            var affected = await db.Users
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteUser")
        .WithOpenApi();

        group.MapGet("/Roles", async (UserContext db) =>
        {
            return await db.Roles.ToListAsync();
        })
        .WithName("GetAllRoles")
        .WithOpenApi();

        group.MapGet("/Roles/{id}", async Task<Results<Ok<Role>, NotFound>> (int id, UserContext db) =>
        {
            return await db.Roles.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Role model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetRolesById")
        .WithOpenApi();
    }
}
