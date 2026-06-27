**Nombre:** Diego José Piox Casasola  
**Carné:** 202405843  
**Curso:** Introducción a la Programación y Computación 2  
**Fecha:** 26 de junio de 2026

**Parte 1. Evaluación Conceptual**

**1. Tabla comparativa de formatos de intercambio**

|             |                                                                                                                                         |                                                                                                |
| ----------- | --------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| **Formato** | **Ventajas**                                                                                                                            | **Desventajas**                                                                                |
| CSV         | Es simple, ligero y rápido de procesar. Compatible con la mayoría de aplicaciones.                                                      | No admite estructuras jerárquicas ni tipos de datos complejos. No posee validación de formato. |
| XML         | Permite representar estructuras complejas y validar la información mediante esquemas. Es muy utilizado para integración entre sistemas. | Consume más espacio, es más lento de procesar y su sintaxis es más extensa que CSV.            |
## 2. Diferencia entre Serialización y Deserialización

La **serialización** consiste en convertir un objeto de C# en un formato de intercambio, como JSON, para almacenarlo o enviarlo por una red.

La **deserialización** realiza el proceso contrario: toma un documento JSON y lo convierte nuevamente en un objeto de C#, permitiendo trabajar con los datos dentro de la aplicación.

En .NET este proceso se realiza mediante la librería **System.Text.Json**, utilizando los métodos `JsonSerializer.Serialize()` y `JsonSerializer.Deserialize()`.

## 3. Antipatrón N+1 y estrategia de Batching

El problema **N+1** ocurre cuando una aplicación realiza una consulta inicial y posteriormente ejecuta una consulta adicional por cada registro obtenido. Esto genera un gran número de accesos a la base de datos y disminuye considerablemente el rendimiento.

La estrategia para evitar este problema es el **Batching**, que consiste en agrupar varios registros en memoria y enviarlos a la base de datos en un solo bloque utilizando métodos como `AddRange()` y una única llamada a `SaveChangesAsync()`. Esto reduce la cantidad de operaciones y mejora el desempeño del sistema.

# Parte 2. Implementación Práctica
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class Alumno
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Carrera { get; set; }
}

public class ApiService
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<Alumno?> ObtenerAlumnoAsync()
    {
        try
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync("https://api.usac.edu/v1/alumnos");

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            Alumno? alumno = JsonSerializer.Deserialize<Alumno>(json, opciones);

            return alumno;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
}



Endpoint para carga masiva CSV


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AlumnosController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlumnosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("carga-masiva")]
    public async Task<IActionResult> CargarArchivo(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("Debe seleccionar un archivo.");

        List<Alumno> alumnos = new List<Alumno>();

        using (var reader = new StreamReader(archivo.OpenReadStream()))
        {
            string? linea;

            await reader.ReadLineAsync();

            while ((linea = await reader.ReadLineAsync()) != null)
            {
                string[] datos = linea.Split(',');

                Alumno alumno = new Alumno
                {
                    Id = int.Parse(datos[0]),
                    Nombre = datos[1],
                    Carrera = datos[2]
                };

                alumnos.Add(alumno);
            }
        }

        await _context.Alumnos.AddRangeAsync(alumnos);
        await _context.SaveChangesAsync();

        return Ok("Carga masiva completada correctamente.");
    }
}

# Parte 3. Referencia Bibliográfica

Facultad de Ingeniería, USAC. (2026). _Sesión 20: Integración de Datos. Consumo de APIs Externas y Carga Masiva (CSV/XML)._ Laboratorio del curso Introducción a la Programación y Computación 2. Guatemala.