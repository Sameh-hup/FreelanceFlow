using University_Academic.Enums;

namespace University_Academic.Entities
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int SectionId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Registered;
        public Grade Grade { get; set; } = null!;
        public Student Student { get; set; } = null!;
        public CourseSection Section { get; set; } = null!;

    }
}
