using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Models.Data;
using Syzoj.Api.Models.Requests;
using Syzoj.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Syzoj.Api.Controllers
{
    [Route("api/discuss")]
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
        public async Task<IActionResult> CreateDiscuss([FromBody]CreateDiscussionRequest req)
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
            };
            dbContext.ForumDiscussions.Add(fd);
            await dbContext.SaveChangesAsync();
            return Ok(new {
                Status = "Success",
                DiscussionEntryId = entry.Id,
            });
        }
        [HttpPost("reply")]
        public async Task<IActionResult> CreateReply([FromBody]CreateReplyRequest req)
        {
            if(!sess.IsAuthenticated())
            {
                return Unauthorized();
            }
            bool canReply = await dbContext.ForumDiscussions.AnyAsync(fd => fd.ForumId == 2 && fd.DiscussionEntryId == req.DiscussionEntryId);
            if(!canReply)
            {
                return Ok(new {
                    Status = "Fail",
                    Message = "Discussion entry does not exist in default forum"
                });
            }
            var entry = new DiscussionReplyEntry() {
                DiscussionEntryId = req.DiscussionEntryId,
                AuthorId = (int) sess.GetAuthenticatedUserId(),
                Content = req.Content,
                TimeCreated = DateTime.Now,
            };
            dbContext.DiscussionReplies.Add(entry);
            await dbContext.SaveChangesAsync();
            return Ok(new {
                Status = "Success"
            });
        }
    }
}