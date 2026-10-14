namespace University_Academic_System.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Code { get; set; }
        public int CreditHours { get; set; }
        public int Level { get; set; }
    }
}