using System.Collections;
using System.Collections.Generic;

namespace AspNetSample.Models
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int Id);

        IEnumerable<Employee> GetAllEmployee();

        Employee Add(Employee emp);

        Employee Update(Employee employeeChanges);
        Employee Delete(int Id);
    }
}