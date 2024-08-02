using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<Note> notes = [
    new Note {
        Id = 1,
        Title = "Note 1",
        Content = "This is Note 1"
    }
];

app.MapGet("/notes", () => 
{
    return TypedResults.Ok(notes);
});

app.MapPost("/notes", (Note note) => 
{
    notes.Add(note);
    return TypedResults.Created("/notes/{id}", note);
});

app.MapGet("/notes/{id}", Results<Ok<Note>, NotFound> (int id) =>
{
    var target = notes.SingleOrDefault(n => n.Id == id);
    return target == null ? TypedResults.NotFound() : TypedResults.Ok(target);
});

app.MapPut("/notes/{id}", Results<Ok<Note>, BadRequest> (int id, Note note) => 
{
    var target = notes.SingleOrDefault(n => n.Id == id);

    if (target == null)
    {
        return TypedResults.BadRequest();
    }

    target.Title = note.Title;
    target.Content = note.Content;

    return TypedResults.Ok(target);
});

app.MapDelete("/notes/{id}", Results<NoContent, BadRequest> (int id) => {
    int index = notes.FindIndex(n => n.Id == id);

    if (index == -1)
    {
        return TypedResults.BadRequest();
    }

    notes.RemoveAt(index);
    return TypedResults.NoContent();    
});

app.Run();

record Note ()
{
    public int Id { get; init; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
}
