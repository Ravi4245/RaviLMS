namespace RaviLMS.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }  // Make sure this exists and spelled exactly

        public string Description { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }

    }
}
