using LoncotesLibrary.Models;
using LoncotesLibrary.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/api/materials", (LoncotesLibraryDbContext db, int? materialTypeId, int? genreId) =>
{
    var query = db.Materials
        .Include(m => m.Genre)
        .Include(m => m.MaterialType)
        .Where(m => m.OutOfCirculationSince == null);

    if (materialTypeId != null)
    {
        query = query.Where(Material => Material.MaterialTypeId == materialTypeId);
    }
    if (genreId != null)
    {
        query = query.Where(Material => Material.GenreId == genreId);
    }

    return query
        .Select(m => new MaterialDTO
        {
            Id = m.Id,
            MaterialName = m.MaterialName,
            MaterialTypeId = m.MaterialTypeId,
            GenreId = m.GenreId,
            OutOfCirculationSince = m.OutOfCirculationSince,
            MaterialType = new MaterialTypeDTO
            {
                Id = m.MaterialType.Id,
                Name = m.MaterialType.Name,
                CheckoutDays = m.MaterialType.CheckoutDays
            },
            Genre = new GenreDTO
            {
                Id = m.Genre.Id,
                Name = m.Genre.Name
            }
        }).ToList();
});

app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Material newMaterial = db.Materials
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .Include(m => m.Checkouts)
    .ThenInclude(c => c.Patron)
    .SingleOrDefault(m => m.Id == id);

    if (newMaterial == null)
    {
        return Results.NotFound();
    }

    MaterialDTO result = new MaterialDTO
    {
        Id = newMaterial.Id,
        MaterialName = newMaterial.MaterialName,
        MaterialTypeId = newMaterial.MaterialTypeId,
        GenreId = newMaterial.GenreId,
        OutOfCirculationSince = newMaterial.OutOfCirculationSince,
        MaterialType = new MaterialTypeDTO
        {
            Id = newMaterial.MaterialType.Id,
            Name = newMaterial.MaterialType.Name,
            CheckoutDays = newMaterial.MaterialType.CheckoutDays
        },
        Genre = new GenreDTO
        {
            Id = newMaterial.Genre.Id,
            Name = newMaterial.Genre.Name
        },
        Checkouts = newMaterial.Checkouts.Select(c => new CheckoutDTO
        {
            Id = c.Id,
            MaterialId = c.MaterialId,
            PatronId = c.PatronId,
            CheckoutDate = c.CheckoutDate,
            ReturnDate = c.ReturnDate,
            Patron = new PatronDTO
            {
                Id = c.Patron.Id,
                FirstName = c.Patron.FirstName,
                LastName = c.Patron.LastName,
                Address = c.Patron.Address,
                Email = c.Patron.Email,
                IsActive = c.Patron.IsActive
            }
        }).ToList()
    };

    return Results.Ok(result);
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

app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes
    .Select(mt => new MaterialTypeDTO
    {
        Id = mt.Id,
        Name = mt.Name,
        CheckoutDays = mt.CheckoutDays
    }).ToList();
});

app.MapGet("/api/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres
    .Select(g => new GenreDTO
    {
        Id = g.Id,
        Name = g.Name
    }).ToList();
});

app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) =>
{
    return db.Patrons
    .Select(p => new PatronDTO
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Address = p.Address,
        Email = p.Email,
        IsActive = p.IsActive
    }).ToList();
});

app.MapGet("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Patron newPatron = db.Patrons
    .Include(p => p.Checkouts)
    .ThenInclude(c => c.Material)
    .ThenInclude(m => m.MaterialType)
    .SingleOrDefault(p => p.Id == id);

    if (newPatron == null)
    {
        return Results.NotFound();
    }

    PatronDTO result = new PatronDTO
    {
        Id = newPatron.Id,
        FirstName = newPatron.FirstName,
        LastName = newPatron.LastName,
        Address = newPatron.Address,
        Email = newPatron.Email,
        IsActive = newPatron.IsActive,
        Checkouts = newPatron.Checkouts.Select(c => new CheckoutDTO
        {
            Id = c.Id,
            MaterialId = c.MaterialId,
            PatronId = c.PatronId,
            CheckoutDate = c.CheckoutDate,
            ReturnDate = c.ReturnDate,
            Material = new MaterialDTO
            {
                Id = c.Material.Id,
                MaterialName = c.Material.MaterialName,
                MaterialTypeId = c.Material.MaterialTypeId,
                GenreId = c.Material.GenreId,
                OutOfCirculationSince = c.Material.OutOfCirculationSince,
                MaterialType = new MaterialTypeDTO
                {
                    Id = c.Material.MaterialType.Id,
                    Name = c.Material.MaterialType.Name,
                    CheckoutDays = c.Material.MaterialType.CheckoutDays
                }
            }
        }).ToList()
    };

    return Results.Ok(result);
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

app.MapPut("/api/checkouts/{id}", (LoncotesLibraryDbContext db, int id, Checkout checkout) =>
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

app.MapGet("/api/materials/available", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    .Where(m => m.OutOfCirculationSince == null)
    .Where(m => m.Checkouts.All(co => co.ReturnDate != null))
    .Select(material => new MaterialDTO
    {
        Id = material.Id,
        MaterialName = material.MaterialName,
        MaterialTypeId = material.MaterialTypeId,
        GenreId = material.GenreId,
        OutOfCirculationSince = material.OutOfCirculationSince
    })
    .ToList();
});

app.MapGet("/api/checkouts/overdue", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts
    .Include(p => p.Patron)
    .Include(co => co.Material)
    .ThenInclude(m => m.MaterialType)
    .Where(co =>
        (DateTime.Today - co.CheckoutDate).Days >
        co.Material.MaterialType.CheckoutDays &&
        co.ReturnDate == null)
        .Select(co => new CheckoutDTO
        {
            Id = co.Id,
            MaterialId = co.MaterialId,
            Material = new MaterialDTO
            {
                Id = co.Material.Id,
                MaterialName = co.Material.MaterialName,
                MaterialTypeId = co.Material.MaterialTypeId,
                MaterialType = new MaterialTypeDTO
                {
                    Id = co.Material.MaterialType.Id,
                    Name = co.Material.MaterialType.Name,
                    CheckoutDays = co.Material.MaterialType.CheckoutDays
                },
                GenreId = co.Material.GenreId,
                OutOfCirculationSince = co.Material.OutOfCirculationSince
            },
            PatronId = co.PatronId,
            Patron = new PatronDTO
            {
                Id = co.Patron.Id,
                FirstName = co.Patron.FirstName,
                LastName = co.Patron.LastName,
                Address = co.Patron.Address,
                Email = co.Patron.Email,
                IsActive = co.Patron.IsActive
            },
            CheckoutDate = co.CheckoutDate,
            ReturnDate = co.ReturnDate
        })
    .ToList();
});

app.Run();