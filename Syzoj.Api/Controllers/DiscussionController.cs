using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Models.Data;
using Syzoj.Api.Models.Requests;
using Syzoj.Api.Services;

namespace Syzoj.Api.Controllers
{
    [Route("api/discuss")]
    public class DiscussionController : ControllerBase
    {
        private readonly ISessionManager sessionManager;
        private readonly ApplicationDbContext dbContext;
        public DiscussionController(ISessionManager sessionManager, ApplicationDbContext dbContext)
        {
            this.sessionManager = sessionManager;
            this.dbContext = dbContext;
        }

        [HttpPost("forum")]
        public async Task<IActionResult> CreateForum()
        {
            Forum f = new Forum();
            dbContext.Forums.Add(f);
            await dbContext.SaveChangesAsync();
            return Ok(new {
                Status = "Success",
                Id = f.Id,
            });
        }
        // TODO: Check for permission to post in forum
        [HttpPost("discussion")]
        public async Task<IActionResult> CreateDiscuss([FromBody]CreateDiscussionRequest req)
        {
            Session sess = await sessionManager.GetSession(req.SessionID);
            if(sess == null || sess.UserId == null)
            {
                return Unauthorized();
            }
            var entry = new DiscussionEntry() {
                Title = req.Title,
                Content = req.Content,
                AuthorId = (int) sess.UserId,
                ForumId = req.ForumId,
                TimeCreated = DateTime.Now,
                TimeModified = DateTime.Now,
                TimeLastReply = DateTime.Now,
            };
            dbContext.Discussions.Add(entry);
            await Task.WhenAll(new[] {
                sessionManager.UpdateSession(sess),
                dbContext.SaveChangesAsync(),
            });
            return Ok(new {
                Status = "Success",
                DiscussionEntryId = entry.Id,
            });
        }
        [HttpPost("reply")]
        public async Task<IActionResult> CreateReply([FromBody]CreateReplyRequest req)
        {
            Session sess = await sessionManager.GetSession(req.SessionId);
            if(sess == null || sess.UserId == null)
            {
                return Unauthorized();
            }
            var entry = new DiscussionReplyEntry() {
                DiscussionEntryId = req.DiscussionEntryId,
                AuthorId = (int) sess.UserId,
                Content = req.Content,
                TimeCreated = DateTime.Now,
            };
            dbContext.DiscussionReplies.Add(entry);
            await Task.WhenAll(new[] {
                sessionManager.UpdateSession(sess),
                dbContext.SaveChangesAsync(),
            });
            return Ok(new {
                Status = "Success"
            });
        }
    }
}