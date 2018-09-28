namespace Syzoj.Api.Problems
{
    public abstract class ProblemsetPermission : Permission<ProblemsetPermission>
    {
        public static ProblemsetPermission ViewPermission = new ProblemsetViewPermission();
        public static ProblemsetPermission EditPermission = new ProblemsetEditPermission();
        
        public ProblemsetPermission(string name, string errorMessage) : base(name, errorMessage) { }
        
        private class ProblemsetViewPermission : ProblemsetPermission
        {
            public ProblemsetViewPermission() : base("view", "syzoj.error.problemsetNotViewable") { }
        }

        private class ProblemsetEditPermission : ProblemsetPermission
        {
            public ProblemsetEditPermission() : base("edit", "syzoj.error.problemsetNotEditable") { }
        }
    }
}