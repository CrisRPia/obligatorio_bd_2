using backend.src.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("departments/")]
public class DepartmentController : Controller
{
    [HttpGet]
    public async Task<List<Department>> GetAllDepartments()
    {
        throw new NotImplementedException();
    }
}
