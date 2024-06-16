using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Services
{
    public static class PostServiceExtensions
    {
        public static async Task<IEnumerable<(int Id, string Title)>> GetUserPosts(this PostService postService, int userId)
        {
            var context = postService.GetContext();
            return await context.Posts
                .Where(p => p.UserId == userId)
                .Select(p => new { p.Id, p.Title })
                .ToListAsync()
                .ContinueWith(task => task.Result.Select(p => (p.Id, p.Title)));
        }
    }
}