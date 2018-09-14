using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public Problem Problem { get; set; }
        public string Path { get; set; }
    }
}