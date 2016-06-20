using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using SelectNPlusOne.Models;
using System.Data.SqlClient;
using Dapper;

namespace SelectNPlusOne.Controllers
{
    public class HomeController : Controller
    {
        private static string connectionString = "Data Source=.;Initial Catalog=SelectNPlusOneTestDB-001;Integrated Security=SSPI";

        /// <summary>
        /// A really bad way to get the data
        /// </summary>
        /// <returns>Asynchronous action result</returns>
        public async Task<IActionResult> Bad()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var blogPosts = await connection.QueryAsync<BlogPost>(@"
                    SELECT * FROM BlogPost");
                foreach (var post in blogPosts)
                {
                    var comments = await connection.QueryAsync<BlogPostComment>(@"
                        SELECT * FROM BlogPostComment
                        WHERE BlogPostId = @BlogPostId",
                        new { BlogPostId = post.BlogPostId });
                    post.CommentCount = comments.Count();
                }
                return View("Index", blogPosts);
            }
        }

        /// <summary>
        /// A bad way to do paging
        /// </summary>
        /// <returns>Asynchronous action result</returns>
        public async Task<IActionResult> BadPaging(int pageNumber = 1, int pageSize = 10)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var blogPosts = await connection.QueryAsync<BlogPost>(@"
                    SELECT
                        bp.BlogPostId,
                        bp.Title,
                        COUNT(bpc.BlogPostCommentId) 'CommentCount'
                    FROM BlogPost bp
                    LEFT JOIN BlogPostComment bpc
                        ON bpc.BlogPostId = bp.BlogPostId
                    GROUP BY bp.BlogPostId, bp.Title");
                // This paging happens in the application and not in the DB!
                return View("Index", blogPosts.OrderByDescending(bp => bp.CommentCount)
                                              .Skip(pageSize * (pageNumber - 1))
                                              .Take(pageSize));
            }
        }

        /// <summary>
        /// Shows the top 10 most commented posts
        /// </summary>
        /// <returns>Asynchronous action result</returns>
        public async Task<IActionResult> TopTen()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var blogPosts = await connection.QueryAsync<BlogPost>(@"
                    SELECT TOP 10
                        bp.BlogPostId,
                        bp.Title,
                        COUNT(bpc.BlogPostCommentId) 'CommentCount'
                    FROM BlogPost bp
                    LEFT JOIN BlogPostComment bpc
                        ON bpc.BlogPostId = bp.BlogPostId
                    GROUP BY bp.BlogPostId, bp.Title
                    ORDER BY COUNT(bpc.BlogPostCommentId) DESC");
                return View("Index", blogPosts);
            }
        }

        /// <summary>
        /// A much better way to get the data
        /// </summary>
        /// <returns>Asynchronous action result</returns>
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var blogPosts = await connection.QueryAsync<BlogPost>(@"
                    SELECT
                        bp.BlogPostId,
                        bp.Title,
                        COUNT(bpc.BlogPostCommentId) 'CommentCount'
                    FROM BlogPost bp
                    LEFT JOIN BlogPostComment bpc
                        ON bpc.BlogPostId = bp.BlogPostId
                    GROUP BY bp.BlogPostId, bp.Title
                    ORDER BY COUNT(bpc.BlogPostCommentId) DESC
                    OFFSET @OffsetRows ROWS
                    FETCH NEXT @LimitRows ROWS ONLY", new
                    {
                        OffsetRows = pageSize * (pageNumber - 1),
                        LimitRows = pageSize
                    });
                return View(blogPosts);
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
