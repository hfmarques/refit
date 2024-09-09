using Refit;

namespace WebApi;

public interface IBlogApi
{
    [Headers("User-Agent: MyAwesomeApp/1.0")]
    [Get("/posts")]
    Task<List<Post>> GetPostsAsync();
    
    [Get("/secure-posts")]
    Task<List<Post>> GetSecurePostsAsync([Header("Authorization")] string bearerToken);
    
    [Get("/user-posts")]
    Task<List<Post>> GetUserPostsAsync([Authorize(scheme: "Bearer")] string token);
    
    [Get("/posts/{id}")]
    Task<HttpResponseMessage> GetPostRawAsync(int id);

    [Get("/posts/{id}")]
    Task<ApiResponse<Post>> GetPostWithMetadataAsync(int id);
    
    [Get("/posts/{id}")]
    Task<Post> GetPostAsync(int id);

    [Post("/posts")]
    Task<ApiResponse<Post>> CreatePostAsync([Body] Post post);

    [Put("/posts/{id}")]
    Task<Post> UpdatePostAsync(int id, [Body] Post post);

    [Delete("/posts/{id}")]
    Task DeletePostAsync(int id);
    
    [Get("/posts")]
    Task<List<Post>> GetPostsAsync([Query] PostQueryParameters parameters);

    [Get("/users/{userId}/posts")]
    Task<List<Post>> GetUserPostsAsync(int userId);
}

public class PostQueryParameters
{
    public int? UserId { get; set; }
    public string? Title { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
}