﻿using System;
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
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public DiscussionController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET /api/discussion/5
        
        /// <summary>
        /// Get discussion by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<JsonResult> GetDiscussion(int id)
        {
            // TODO: EF Core does not support recursive include. We should implement it ourselves.
            var discussions = dbContext.Discussions
                .Include(d => d.Author)
                .Include(d => d.Replies);
            var discussionEntry = await discussions.FirstOrDefaultAsync(d => d.Id == id);
            
            return new JsonResult(new
            {
                status = JsonStatusCode.Success,
                result = discussionEntry,
            });
        }

        // PUT /api/discussion
        
        /// <summary>
        /// Create a new discussion
        /// </summary>
        [HttpPut]
        public async Task<JsonResult> PutDiscussion(DiscussionCreateApiModel entryModel)
        {
            var entry = new DiscussionEntry();

            // TODO: Read author from authentication.
            // entry.Author = await dbContext.Users.FindAsync(...);
            entry.Content = entryModel.Content;
            if (entryModel.ShowInBoard is bool showInBoard)
                entry.ShowInBoard = showInBoard;
            
            await dbContext.Discussions.AddAsync(entry);
            await dbContext.SaveChangesAsync();
            
            return new JsonResult(new { status = JsonStatusCode.Success });
        }

        // PATCH /api/discussion
        
        /// <summary>
        /// Change a discussion
        /// </summary>
        [HttpPatch]
        public async Task<JsonResult> PatchContent(DiscussionApiModel changes)
        {
            var original = await dbContext.Discussions.FindAsync(changes.Id);
            
            if (changes.Content != null)
                original.Content = changes.Content;
            
            if (changes.ShowInBoard is bool nonNullOption)
                original.ShowInBoard = nonNullOption;
            
            await dbContext.SaveChangesAsync();
            
            return new JsonResult(new { status = JsonStatusCode.Success });
        }
    }
}
