using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Practice21.MinimalAPI.Data;
using Practice21.MinimalAPI.Models;
using Microsoft.AspNetCore.Authorization;
namespace Practice21.MinimalAPI;

public static class PhoneBookEntryEndpoints
{
    public static void MapPhoneBookEntryEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/PhoneBookEntry").WithTags(nameof(PhoneBookEntry));

        group.MapGet("/", async (PhoneBookContext db) =>
        {
            return await db.PhoneBookEntries.ToListAsync();
        })
        .WithName("GetAllPhoneBookEntries")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<PhoneBookEntry>, NotFound>> (int id, PhoneBookContext db) =>
        {
            return await db.PhoneBookEntries.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is PhoneBookEntry model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetPhoneBookEntryById")
        .WithOpenApi();

        group.MapPut("/{id}", [Authorize(Roles = "admin")] async Task<Results<Ok, NotFound>> (int id, PhoneBookEntry phoneBookEntry, PhoneBookContext db) =>
        {
            var affected = await db.PhoneBookEntries
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, phoneBookEntry.Id)
                    .SetProperty(m => m.LastName, phoneBookEntry.LastName)
                    .SetProperty(m => m.FirstName, phoneBookEntry.FirstName)
                    .SetProperty(m => m.MiddleName, phoneBookEntry.MiddleName)
                    .SetProperty(m => m.PhoneNumber, phoneBookEntry.PhoneNumber)
                    .SetProperty(m => m.Address, phoneBookEntry.Address)
                    .SetProperty(m => m.Description, phoneBookEntry.Description)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdatePhoneBookEntry")
        .WithOpenApi();

        group.MapPost("/", [Authorize(Roles = "admin, user")] async (PhoneBookEntry phoneBookEntry, PhoneBookContext db) =>
        {
            db.PhoneBookEntries.Add(phoneBookEntry);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/PhoneBookEntry/{phoneBookEntry.Id}",phoneBookEntry);
        })
        .WithName("CreatePhoneBookEntry")
        .WithOpenApi();

        group.MapDelete("/{id}", [Authorize(Roles = "admin")] async Task<Results<Ok, NotFound>> (int id, PhoneBookContext db) =>
        {
            var affected = await db.PhoneBookEntries
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeletePhoneBookEntry")
        .WithOpenApi();
    }
}
