using backend.src.Models;
using backend.src.Queries;
using Microsoft.AspNetCore.Mvc;

namespace backend.src.Controllers;

[ApiController]
[Route("departments/")]
public class DepartmentController : Controller
{
    [HttpGet]
    public async Task<IEnumerable<Department>> GetAllDepartments()
    {
        var rows = await DB.Queries.GetDepartments();

        return rows.Select(row => new Department()
        {
            Name = row.Name,
            DepartmentId = new Ulid(row.DepartmentId),
        });
    }

    private readonly string[] departments =
    [
        "Artigas",
        "Canelones",
        "Cerro Largo",
        "Colonia",
        "Durazno",
        "Flores",
        "Florida",
        "Lavalleja",
        "Maldonado",
        "Montevideo",
        "Paysandú",
        "Río Negro",
        "Rivera",
        "Rocha",
        "Salto",
        "San José",
        "Soriano",
        "Tacuarembó",
        "Treinta y Tres",
    ];

    [HttpPost]
    public async Task<IEnumerable<Department>> InitDepartments()
    {
        if ((await GetAllDepartments()).Any()) {
            throw new InvalidOperationException();
        }

        var departments = this.departments.Select(name => new Department
        {
            DepartmentId = Ulid.NewUlid(),
            Name = name,
        });

        foreach (var department in departments)
        {
            await DB.Queries.InsertDepartment(
                new()
                {
                    DepartmentId = department.DepartmentId.ToByteArray(),
                    Name = department.Name,
                }
            );
        }

        return await GetAllDepartments();
    }
}
