using System.Collections.Generic;

namespace Syzoj.Api.Mvc
{
    public class CustomPagedResponse<TResult> : CustomResponse<IEnumerable<TResult>>
    {
        public PaginationInfo PageInfo { get; set; }
        public CustomPagedResponse() { }
        public CustomPagedResponse(IEnumerable<TResult> result) : base(result) { }
    }
}