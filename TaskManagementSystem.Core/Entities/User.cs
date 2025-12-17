using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementSystem.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Task> CreatedTasks { get; set; }
        public virtual ICollection<Task> AssignedTasks { get; set; }
    }

    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
        Employee = 3
    }
}
