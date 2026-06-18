using Microsoft.AspNetCore.Mvc;
using ControlAcademicoMvc.Models;

namespace ControlAcademicoMvc.Controllers;

public class EstudianteController : Controller
{
    private static readonly List<Estudiante> _baseDatosMemoria = new()
    {
        new Estudiante
        {
            Carne = 2026012,
            Nombre = "Fernando Velásquez",
            Promedio = 91.5
        },
        new Estudiante
        {
            Carne = 2026045,
            Nombre = "María Mercedes",
            Promedio = 84.0
        }
    };

    public IActionResult Listar()
    {
        return View(_baseDatosMemoria);
    }

    [HttpPost]
    public IActionResult Registrar([FromBody] Estudiante nuevoEstudiante)
    {
        if (nuevoEstudiante.Carne <= 0 ||
            string.IsNullOrEmpty(nuevoEstudiante.Nombre))
        {
            return BadRequest(new
            {
                mensaje = "Datos del estudiante inválidos."
            });
        }

        _baseDatosMemoria.Add(nuevoEstudiante);

        return Created(
            $"/Estudiante/Historial/{nuevoEstudiante.Carne}",
            nuevoEstudiante
        );
    }
}