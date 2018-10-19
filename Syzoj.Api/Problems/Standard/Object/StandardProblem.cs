using System;
using System.Threading.Tasks;
using Syzoj.Api.Data;
using Syzoj.Api.Interfaces;
using Syzoj.Api.Interfaces.View;
using Syzoj.Api.Object;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems.Standard.Object
{
    public class StandardProblem : IProblemObject, IProblemObjectAcceptingContract<IViewProblemsetContract, IViewProblemContract>, IViewProblemContract
    {
        private readonly IServiceProvider provider;
        private readonly ApplicationDbContext dbContext;
        private readonly ProblemDbModel model;
        private readonly Uri uri;

        internal StandardProblem(IServiceProvider provider, ApplicationDbContext dbContext, ProblemDbModel model)
        {
            this.provider = provider;
            this.dbContext = dbContext;
            this.model = model;
            this.uri = new Uri($"object:///Syzoj.Api/problem-standard/${model.Id}");
        }

        Task<IViewProblemContract> IProblemObjectAcceptingContract<IViewProblemsetContract, IViewProblemContract>.CreateContract(IViewProblemsetContract c)
        {
            return Task.FromResult<IViewProblemContract>(this);
        }

        public Uri GetUri() => uri;

        Task<string> IViewProblemContract.GetProblemDefaultTitle()
        {
            return Task.FromResult<string>(model.Title);
        }

        Task<ViewModel> IViewProblemContract.GetProblemContent()
        {
            var content = new ViewModel()
            {
                ComponentName = "problem-standard",
                Content = model.Content
            };
            return Task.FromResult<ViewModel>(content);
        }
    }
}