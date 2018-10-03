using System.Collections.Generic;

namespace Syzoj.Api.Mvc
{
    public class CustomPagedResponse<TResult> : CustomResponse<IEnumerable<TResult>>
    {
        public PaginationInfo PageInfo { get; set; }
    }
}