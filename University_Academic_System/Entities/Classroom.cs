namespace University_Academic_System.Entities
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Building { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}