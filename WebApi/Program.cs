using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Refit;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { });

builder.Services
    .AddRefitClient<IBlogApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new("https://jsonplaceholder.typicode.com"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    app.Map("/", () => Results.Redirect("/swagger"));
}

app.MapGet("/posts/{id:int}", async (int id, IBlogApi api) =>
    Results.Ok(await api.GetPostAsync(id)));

app.MapGet("/posts", async (IBlogApi api) =>
    Results.Ok(await api.GetPostsAsync()));

app.MapGet("/posts/readAsStringAsync/{id:int}", async (int id, IBlogApi api) =>
{
    var response = await api.GetPostRawAsync(id);

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        var post = JsonSerializer.Deserialize<Post>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Console.WriteLine($"Retrieved post: {post!.Title}");
        return Results.Ok(post);
    }

    Console.WriteLine($"Error: {response.StatusCode}");
    return Results.BadRequest($"Error: {response.StatusCode}");
});

app.MapGet("/posts/getPostWithMetadataAsync/{id:int}", async (int id, IBlogApi api) =>
{
    var response = await api.GetPostWithMetadataAsync(id);

    if (!response.IsSuccessStatusCode) return Results.NotFound();
    
    var post = response.Content;
    return Results.Ok(post);

});

app.MapPost("/posts", async ([FromBody] Post post, IBlogApi api) =>
{
    var createResponse = await api.CreatePostAsync(post);

    if (createResponse.IsSuccessStatusCode)
    {
        var createdPost = createResponse.Content;
        var locationHeader = createResponse.Headers.Location;
        Console.WriteLine($"Created post with ID: {createdPost.Id}");
        Console.WriteLine($"Location: {locationHeader}");
        return Results.Created(locationHeader, createdPost);
    }

    Console.WriteLine($"Error: {createResponse.Error.Content}");
    Console.WriteLine($"Status: {createResponse.StatusCode}");
    return Results.BadRequest($"Error: {createResponse.Error.Content} Status: {createResponse.StatusCode}");
});

app.MapPut("/posts/{id:int}", async (int id, [FromBody] Post post, IBlogApi api) =>
await api.UpdatePostAsync(id, post));

app.MapDelete("/posts/{id:int}", async (int id, IBlogApi api) =>
    await api.DeletePostAsync(id));

app.Run();