using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int DeptId { get; set; }
        public Department Department { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; }
        public ICollection<Property> Properties { get; set; }
    }
}
