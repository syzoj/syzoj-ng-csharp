using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Models.Data;
using Syzoj.Api.Models.Requests;
using Syzoj.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Z.EntityFramework.Plus;
using Syzoj.Api.Filters;
using Syzoj.Api.Models.Responses;

namespace Syzoj.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/discuss")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISessionManager sess;
        public DiscussionController(ApplicationDbContext dbContext, ISessionManager sess)
        {
            this.dbContext = dbContext;
            this.sess = sess;
        }
        /// <summary>
        /// Creates a new discussion entry in public forum.
        /// </summary>
        /// <response code="200">
        /// Possible error codes:
        /// - 0: Success.
        /// </response>
        [HttpPost("discussion")]
        [ValidateModel]
        [RequiresLogin]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        [ProducesResponseType(typeof(DiscussionEntryCreatedResponse), 200)]
        public async Task<IActionResult> CreateDiscuss([FromBody] CreateDiscussionRequest req)
        {
            var entry = new DiscussionEntry() {
                Title = req.Title,
                Content = req.Content,
                AuthorId = (int) sess.GetAuthenticatedUserId(),
                TimeCreated = DateTime.Now,
                TimeModified = DateTime.Now,
                TimeLastReply = DateTime.Now,
            };
            dbContext.Discussions.Add(entry);
            var fd = new ForumDiscussion() {
                ForumId = 2,
                DiscussionEntryId = entry.Id,
                TimeLastReply = DateTime.Now,
            };
            dbContext.ForumDiscussions.Add(fd);
            await dbContext.SaveChangesAsync();
            return Ok(new DiscussionEntryCreatedResponse() {
                DiscussionEntryId = entry.Id,
            });
        }

        /// <summary>
        /// Creates a reply for the given discussion entry.
        /// </summary>
        /// <response code="200">
        /// Possible error codes:
        /// - 0: Success.
        /// - 2001: Discussion entry does not exist in public forum.
        /// </response>
        [HttpPost("reply")]
        [ValidateModel]
        [RequiresLogin]
        [ProducesResponseType(typeof(BaseResponse), 200)]
        [ProducesResponseType(typeof(DiscussionReplyEntryCreatedResponse), 200)]
        public async Task<IActionResult> CreateReply([FromBody] CreateReplyRequest req)
        {
            var discussion = await dbContext.ForumDiscussions
                .Where(fd => fd.ForumId == 2 && fd.DiscussionEntryId == req.DiscussionEntryId)
                .Include(fd => fd.DiscussionEntry)
                .Select(fd => fd.DiscussionEntry)
                .SingleOrDefaultAsync();
            if(discussion == null)
            {
                return Ok(new BaseResponse() {
                    Status = "Fail",
                    Code = 2001,
                    Message = "Discussion entry does not exist in default forum"
                });
            }

            var currentTime = DateTime.Now;
            discussion.TimeLastReply = currentTime;
            
            var entry = new DiscussionReplyEntry() {
                DiscussionEntryId = req.DiscussionEntryId,
                AuthorId = (int) sess.GetAuthenticatedUserId(),
                Content = req.Content,
                TimeCreated = DateTime.Now,
            };
            dbContext.DiscussionReplies.Add(entry);

            // EF Core does not support batch updating dong at server side, using Z.EntityFramework.Plus.EFCore for this purpose
            // Note that SQL statements are sent immediately so they must be wrapped in a transaction
            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                await dbContext.ForumDiscussions.Where(fd => fd.DiscussionEntryId == req.DiscussionEntryId).UpdateAsync(fd => new ForumDiscussion() { TimeLastReply = currentTime });
                await dbContext.SaveChangesAsync();
                transaction.Commit();
            }
            return Ok(new DiscussionReplyEntryCreatedResponse() {
                Status = "Success",
                Code = 0,
                DiscussionReplyEntryId = entry.Id,
            });
        }

        /// <summary>
        /// Gets all discussion entries in public forum.
        /// </summary>
        /// <response code="200">
        /// Possible error codes:
        /// - 0: Success.
        /// </response>
        [HttpGet("discussion")]
        [ProducesResponseType(typeof(DiscussionEntryListResponse), 200)]
        public async Task<IActionResult> GetForum()
        {
            var discussions = dbContext.ForumDiscussions
                .Where(fd => fd.ForumId == 2)
                .OrderByDescending(fd => fd.TimeLastReply)
                .Include(fd => fd.DiscussionEntry)
                .Select(fd => fd.DiscussionEntry)
                .Include(d => d.Author)
                .Select(d => new DiscussionEntryResponse() {
                    Id = d.Id,
                    Title = d.Title,
                    AuthorId = d.AuthorId,
                    AuthorName = d.Author.UserName,
                    TimeCreated = d.TimeCreated,
                    TimeLastReply = d.TimeLastReply,
                    TimeModified = d.TimeModified,
                })
                .AsEnumerable();
            return Ok(new DiscussionEntryListResponse() {
                DiscussionEntries = discussions,
            });
        }

        /// <summary>
        /// Gets a discussion entry in public forum.
        /// </summary>
        /// <response code="200">
        /// Possible error codes:
        /// - 0: Success.
        /// </response>
        /// <response code="404">
        /// The discussion entry with the specified ID does not exist in public forum.
        /// </response>
        [HttpGet("discussion/{id}")]
        [ProducesResponseType(typeof(DiscussionEntrySingleResponse), 200)]
        [ProducesResponseType(typeof(BaseResponse), 404)]
        public async Task<IActionResult> GetDiscussionEntry(int id)
        {
            var discussionEntry = await dbContext.ForumDiscussions
                .Where(fd => fd.ForumId == 2 && fd.DiscussionEntryId == id)
                .OrderBy(fd => fd.TimeLastReply)
                .Include(fd => fd.DiscussionEntry)
                .Select(fd => fd.DiscussionEntry)
                .Include(d => d.Author)
                .Include(d => d.Replies)
                .ThenInclude(r => r.Author)
                .Select(d => new DiscussionEntryResponse() {
                    Id = d.Id,
                    Title = d.Title,
                    AuthorId = d.AuthorId,
                    AuthorName = d.Author.UserName,
                    TimeCreated = d.TimeCreated,
                    TimeLastReply = d.TimeLastReply,
                    TimeModified = d.TimeModified,
                    Replies = d.Replies.OrderBy(r => r.TimeCreated).Select(r => new DiscussionReplyEntryResponse() {
                        AuthorId = r.AuthorId,
                        AuthorName = r.Author.UserName,
                        TimeCreated = r.TimeCreated,
                        Content = r.Content,
                    })
                })
                .FirstOrDefaultAsync();
            if(discussionEntry == null)
            {
                return NotFound(new BaseResponse() {
                    Status = "Fail",
                    Message = "Discussion entry does not exist in default forum",
                });
            }
            else
            {
                return Ok(new DiscussionEntrySingleResponse() {
                    Status = "Success",
                    DiscussionEntry = discussionEntry
                });
            }
        }
    }
}