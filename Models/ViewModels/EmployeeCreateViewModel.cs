using Microsoft.AspNetCore.Http;

namespace AspNetSample.Models.ViewModels
{
    public class EmployeeCreateViewModel
    {
    public string Name { get; set; }
    public string Email { get; set; }
    public Dept? Department { get; set; }
    public IFormFile Photo { get; set; }
    }
}