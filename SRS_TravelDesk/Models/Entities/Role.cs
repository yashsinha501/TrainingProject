namespace SRS_TravelDesk.Models.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // Admin, Employee, Manager,  TravelHr,

        public ICollection<User> Users { get; set; }
    }

}
