using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public class Department
    {
        [Key]
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
