using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Models
{
    [DbModel]
    public class ApplicationUser : IdentityUser
    {
    }
}