namespace Syzoj.Api.Problems.Permission
{
    public abstract class SubmissionPermission : Permission<SubmissionPermission>
    {
        public static SubmissionPermission ViewPermission = new SubmissionViewPermission();
        
        public SubmissionPermission(string name, string errorMessage) : base(name, errorMessage) { }
        
        private class SubmissionViewPermission : SubmissionPermission
        {
            public SubmissionViewPermission() : base("view", "syzoj.error.problemsetSubmissionNotViewable") { }
        }
    }
}