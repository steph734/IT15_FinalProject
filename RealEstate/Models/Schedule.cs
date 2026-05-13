using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
