using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syzoj.Api.Data;
using Syzoj.Api.Filters;
using Syzoj.Api.Models;

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
        [HttpGet("{id}")]
        public async Task<DiscussionEntry> GetDiscussion(int id)
        {
            var result = await dbContext.Discussions.FindAsync(id);
            return result;
        }

        // PUT /api/discussion
        [HttpPut]
        public async Task<DiscussionEntry> PutDiscussion(DiscussionEntry entry)
        {
            await dbContext.Discussions.AddAsync(entry);
            return entry;
        }
    }
}
