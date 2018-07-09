using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;
using Syzoj.Api.Filters;
using Syzoj.Api.Models;
using Syzoj.Api.Utils;

namespace Syzoj.Api.Controllers
{
    [Route("api/discussion/reply")]
    [ApiController]
    public class DiscussionReplyController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public DiscussionReplyController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET /api/discussion/reply/5
        
        /// <summary>
        /// Get discussion reply by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<JsonResult> GetDiscussionReply(int id)
        {
            // TODO: EF Core does not support recursive include. We should implement it ourselves.
            var reply = await dbContext.Replies.FindAsync(id);
            
            return new JsonResult(new
            {
                status = JsonStatusCode.Success,
                result = reply,
            });
        }

        // PUT /api/discussion/reply
        
        /// <summary>
        /// Reply a discussion
        /// </summary>
        [HttpPut]
        public async Task<JsonResult> PutDiscussionReply(ReplyApiModel replyModel)
        {
            var entry = new ReplyEntry
            {
                // TODO: Read author from authentication.
                // Author = await dbContext.Users.FindAsync(...);
                Content = replyModel.Content,
            };

            var discussions = dbContext.Discussions.Include(d => d.Replies);
            var discussion = await discussions.FirstAsync(d => d.Id == replyModel.DiscussionId);
            discussion.Replies.Add(entry);
            
            await dbContext.SaveChangesAsync();
            
            return new JsonResult(new
            {
                status = JsonStatusCode.Success,
                id = entry.Id,
            });
        }

        // PUT /api/discussion/subreply
        
        /// <summary>
        /// Create a sub reply
        /// </summary>
        // TODO: Route is incorrect.
        [HttpPut("subreply")]
        public async Task<JsonResult> PutSubReply(SubReplyApiModel replyModel)
        {
            var entry = new ReplyEntry
            {
                // TODO: Read author from authentication.
                // Author = await dbContext.Users.FindAsync(...);
                Content = replyModel.Content,
            };

            var replies = dbContext.Replies.Include(d => d.Replies);
            var reply = await replies.FirstAsync(d => d.Id == replyModel.ReplyId);
            reply.Replies.Add(entry);
            
            await dbContext.SaveChangesAsync();
            
            return new JsonResult(new { status = JsonStatusCode.Success });
        }

        // PATCH /api/discussion/reply
        
        /// <summary>
        /// Change a reply
        /// </summary>
        [HttpPatch]
        public async Task<JsonResult> PatchReply(ReplyChangeApiModel changes)
        {
            var original = await dbContext.Replies.FindAsync(changes.Id);
            
            if (changes.Content != null)
                original.Content = changes.Content;
            
            await dbContext.SaveChangesAsync();
            
            return new JsonResult(new { status = JsonStatusCode.Success });
        }
    }
}
