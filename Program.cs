using LoncotesLibrary.Models;
using LoncotesLibrary.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;

var builder = WebApplication.CreateBuilder(args);

//Configure AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db, IMapper mapper) =>
{
    return db.MaterialTypes.ProjectTo<MaterialDTO>(mapper.ConfigurationProvider).ToList();
});

app.MapGet("/api/genres", (LoncotesLibraryDbContext db, IMapper mapper) =>
{
    return db.Genres.ProjectTo<GenreDTO>(mapper.ConfigurationProvider).ToList();
});

app.MapGet("/api/patrons", (LoncotesLibraryDbContext db, IMapper mapper) =>
{
    return db.Patrons.ProjectTo<PatronDTO>(mapper.ConfigurationProvider).ToList();
});

app.MapGet("/api/materials", (LoncotesLibraryDbContext db, IMapper mapper, int? materialTypeId, int? genreId) =>
{
    var query = db.Materials.Where(m => m.OutOfCirculationSince == null);

    if (materialTypeId != null)
    {
        query = query.Where(Material => Material.MaterialTypeId == materialTypeId);
    }
    if (genreId != null)
    {
        query = query.Where(Material => Material.GenreId == genreId);
    }

    return query.ProjectTo<MaterialDTO>(mapper.ConfigurationProvider).ToList();
});

app.MapGet("/api/materials/{id}", (IMapper mapper, LoncotesLibraryDbContext db, int id) =>
{
    var material = db.Materials
    .ProjectTo<MaterialDTO>(mapper.ConfigurationProvider)
    .SingleOrDefault(m => m.Id == id);

    return material != null ? Results.Ok(material) : Results.NotFound();
});

app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material material) =>
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/api/materials/{material.Id}", material);
});

app.MapDelete("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Material material = db.Materials.SingleOrDefault(material => material.Id == id);
    if (material == null)
    {
        return Results.NotFound();
    }

    material.OutOfCirculationSince = DateTime.Now;
    db.SaveChanges();
    return Results.NoContent();
});

app.MapGet("/api/patrons/{id}", (IMapper mapper, LoncotesLibraryDbContext db, int id) =>
{
    var patron = db.Patrons
    .ProjectTo<PatronDTO>(mapper.ConfigurationProvider)
    .SingleOrDefault(p => p.Id == id);

    return patron != null ? Results.Ok(patron) : Results.NotFound();
});

app.MapPut("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id, Patron patron) =>
{
    Patron patronToUpdate = db.Patrons.SingleOrDefault(patron => patron.Id == id);
    if (patronToUpdate == null)
    {
        return Results.NotFound();
    }
    patronToUpdate.Address = patron.Address;
    patronToUpdate.Email = patron.Email;

    db.SaveChanges();

    return Results.NoContent();
});

app.MapDelete("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Patron patron = db.Patrons.SingleOrDefault(patron => patron.Id == id);
    if (patron == null)
    {
        return Results.NotFound();
    }

    patron.IsActive = false;
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPost("/api/checkouts", (LoncotesLibraryDbContext db, Checkout newCheckout) =>
{
    newCheckout.CheckoutDate = DateTime.Today;
    db.Checkouts.Add(newCheckout);
    db.SaveChanges();
    return Results.Created($"/api/checkouts/{newCheckout.Id}", newCheckout);
});

app.MapPut("/api/checkouts/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Checkout checkoutToUpdate = db.Checkouts.SingleOrDefault(checkout => checkout.Id == id);
    if (checkoutToUpdate == null)
    {
        return Results.NotFound();
    }
    checkoutToUpdate.ReturnDate = DateTime.Today;

    db.SaveChanges();

    return Results.NoContent();
});

app.MapGet("/api/materials/available", (LoncotesLibraryDbContext db, IMapper mapper) =>
{
    var query = db.Materials
    .Where(m => m.OutOfCirculationSince == null)
    .Where(m => m.Checkouts.All(co => co.ReturnDate != null));

    return query.ProjectTo<MaterialDTO>(mapper.ConfigurationProvider).ToList();
});

app.MapGet("/api/checkouts", (LoncotesLibraryDbContext db, IMapper mapper, int? patronId, int? materialId) =>
{
    var query = db.Checkouts
    .Where(c => patronId == null || c.PatronId == patronId)
    .Where(c => materialId == null || c.MaterialId == materialId);

    return query.ProjectTo<CheckoutDTO>(mapper.ConfigurationProvider).ToList();
});

app.MapGet("/api/checkouts/overdue", (LoncotesLibraryDbContext db, IMapper mapper) =>
{
    var query = db.Checkouts.
    Where(co =>
        (DateTime.Today - co.CheckoutDate).Days >
        co.Material.MaterialType.CheckoutDays &&
        co.ReturnDate == null);

    return query.ProjectTo<CheckoutDTO>(mapper.ConfigurationProvider).ToList();
});

app.Run();