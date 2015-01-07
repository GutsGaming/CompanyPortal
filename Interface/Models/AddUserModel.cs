using System.Linq;
using Logic;

namespace Interface.Models
{
    public class AddUserModel
    {
        public IQueryable<Employee> Supervisors;
    }
}