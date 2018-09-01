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
        // TODO: Check for permission to post in forum
        [HttpPost("discussion")]
        [ValidateModel]
        public async Task<IActionResult> CreateDiscuss([FromBody] CreateDiscussionRequest req)
        {
            if(!sess.IsAuthenticated())
            {
                return Unauthorized();
            }
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
            return Ok(new {
                Status = "Success",
                DiscussionEntryId = entry.Id,
            });
        }

        [HttpPost("reply")]
        [ValidateModel]
        public async Task<IActionResult> CreateReply([FromBody] CreateReplyRequest req)
        {
            if(!sess.IsAuthenticated())
            {
                return Unauthorized();
            }
            var discussion = await dbContext.ForumDiscussions
                .Where(fd => fd.ForumId == 2 && fd.DiscussionEntryId == req.DiscussionEntryId)
                .Include(fd => fd.DiscussionEntry)
                .Select(fd => fd.DiscussionEntry)
                .SingleOrDefaultAsync();
            if(discussion == null)
            {
                return Ok(new {
                    Status = "Fail",
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
            return Ok(new {
                Status = "Success"
            });
        }

        [HttpGet("discussion")]
        public async Task<IActionResult> GetForum()
        {
            var discussions = await dbContext.ForumDiscussions
                .Where(fd => fd.ForumId == 2)
                .OrderByDescending(fd => fd.TimeLastReply)
                .Include(fd => fd.DiscussionEntry)
                .Select(fd => fd.DiscussionEntry)
                .Include(d => d.Author)
                .Select(d => new {
                    Id = d.Id,
                    Title = d.Title,
                    AuthorId = d.AuthorId,
                    AuthorName = d.Author.UserName,
                    TimeCreated = d.TimeCreated,
                    TimeLastReply = d.TimeLastReply,
                    TimeModified = d.TimeModified,
                })
                .ToListAsync();
            return Ok(new {
                Status = "Success",
                DiscussionEntries = discussions,
            });
        }
        
        [HttpGet("discussion/{id}")]
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
                .Select(d => new {
                    Id = d.Id,
                    Title = d.Title,
                    AuthorId = d.AuthorId,
                    AuthorName = d.Author.UserName,
                    TimeCreated = d.TimeCreated,
                    TimeLastReply = d.TimeLastReply,
                    TimeModified = d.TimeModified,
                    Replies = d.Replies.OrderBy(r => r.TimeCreated).Select(r => new {
                        AuthorName = r.Author.UserName,
                        TimeCreated = r.TimeCreated,
                        Content = r.Content,
                    })
                })
                .FirstOrDefaultAsync();
            if(discussionEntry == null)
            {
                return Ok(new {
                    Status = "Fail",
                    Message = "Discussion entry does not exist in default forum",
                });
            }
            else
            {
                return Ok(new {
                    Status = "Success",
                    DiscussionEntry = discussionEntry
                });
            }
        }
    }
}