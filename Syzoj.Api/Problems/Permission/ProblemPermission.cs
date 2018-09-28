namespace Syzoj.Api.Problems.Permission
{
    public abstract class ProblemPermission : Permission<ProblemPermission>
    {
        public static ProblemPermission ViewPermission = new ProblemViewPermission();
        public static ProblemPermission EditPermission = new ProblemEditPermission();
        public static ProblemPermission ExportPermission = new ProblemExportPermission();
        public static ProblemPermission SubmitPermission = new ProblemSubmitPermission();
        
        public ProblemPermission(string name, string errorMessage) : base(name, errorMessage) { }

        private class ProblemViewPermission : ProblemPermission
        {
            public ProblemViewPermission() : base("view", "syzoj.error.problemsetProblemNotViewable") { }
        }

        private class ProblemEditPermission : ProblemPermission
        {
            public ProblemEditPermission() : base("edit", "syzoj.error.problemsetProblemNotEditable") { }
        }

        private class ProblemExportPermission : ProblemPermission
        {
            public ProblemExportPermission() : base("export", "syzoj.error.problemsetProblemNotExportable") { }
        }

        private class ProblemSubmitPermission : ProblemPermission
        {
            public ProblemSubmitPermission() : base("submit", "syzoj.error.problemsetProblemNotSubmittable") { }
        }
    }
}