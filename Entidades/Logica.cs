using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.CapaDatos;
using Entidades.CapaEntidades;

namespace Entidades
{
    internal class Logica
    {
        using CapaDatos;
using CapaEntidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaLogica
    {
        public class NotasService
        {
            private readonly MatriculaRepository _matriculaRepository;
            private readonly EstudianteRepository _estudianteRepository;
            private readonly CursoRepository _cursoRepository;

            public NotasService()
            {
                _matriculaRepository = new MatriculaRepository();
                _estudianteRepository = new EstudianteRepository();
                _cursoRepository = new CursoRepository();
            }

            public string RegistrarMatricula(int estudianteId, int cursoId, string periodo)
            {
                // Verificar que exista el estudiante
                var estudiante = _estudianteRepository.GetById(estudianteId);
                if (estudiante == null)
                {
                    return "Error: El estudiante no existe";
                }

                // Verificar que exista el curso
                var curso = _cursoRepository.GetById(cursoId);
                if (curso == null)
                {
                    return "Error: El curso no existe";
                }

                // Verificar que no exista ya una matrícula para este estudiante, curso y periodo
                var matriculas = _matriculaRepository.Read();
                if (matriculas.Any(m => m.Estudiante.Id == estudianteId &&
                                       m.Curso.Id == cursoId &&
                                       m.Periodo == periodo))
                {
                    return "Error: El estudiante ya está matriculado en este curso para este periodo";
                }

                // Crear y guardar la matrícula
                Matricula matricula = new Matricula
                {
                    Estudiante = estudiante,
                    Curso = curso,
                    Periodo = periodo,
                    Estado = EstadoMatricula.Cursando
                };

                if (_matriculaRepository.Save(matricula))
                {
                    return "Matrícula registrada exitosamente";
                }
                return "Error al registrar la matrícula";
            }

            public string RegistrarNotas(int matriculaId, double notaPrimerCorte, double notaSegundoCorte, double notaTercerCorte)
            {
                // Verificar que exista la matrícula
                var matricula = _matriculaRepository.GetById(matriculaId);
                if (matricula == null)
                {
                    return "Error: La matrícula no existe";
                }

                // Validar notas (entre 0 y 5)
                if (notaPrimerCorte < 0 || notaPrimerCorte > 5 ||
                    notaSegundoCorte < 0 || notaSegundoCorte > 5 ||
                    notaTercerCorte < 0 || notaTercerCorte > 5)
                {
                    return "Error: Las notas deben estar entre 0 y 5";
                }

                // Actualizar notas
                matricula.NotaPrimerCorte = notaPrimerCorte;
                matricula.NotaSegundoCorte = notaSegundoCorte;
                matricula.NotaTercerCorte = notaTercerCorte;

                // Actualizar estado si tiene todas las notas
                if (notaPrimerCorte > 0 && notaSegundoCorte > 0 && notaTercerCorte > 0)
                {
                    matricula.Estado = matricula.EstaAprobado() ? EstadoMatricula.Aprobado : EstadoMatricula.Reprobado;
                }

                if (_matriculaRepository.Save(matricula))
                {
                    return "Notas registradas exitosamente";
                }
                return "Error al registrar las notas";
            }

            public double CalcularNotaFinal(int matriculaId)
            {
                var matricula = _matriculaRepository.GetById(matriculaId);
                if (matricula == null)
                {
                    throw new Exception("La matrícula no existe");
                }

                return matricula.CalcularNotaFinal();
            }

            public string DeterminarEstadoMateria(int matriculaId)
            {
                var matricula = _matriculaRepository.GetById(matriculaId);
                if (matricula == null)
                {
                    return "Error: La matrícula no existe";
                }

                return matricula.ObtenerEstadoActual();
            }

            public double CalcularPromedioEstudiante(int estudianteId, string periodo)
            {
                var matriculas = _matriculaRepository.GetByEstudianteYPeriodo(estudianteId, periodo);
                if (!matriculas.Any())
                {
                    return 0;
                }

                double sumaNotas = 0;
                int cantidadMaterias = 0;

                foreach (var matricula in matriculas)
                {
                    // Solo considerar materias con todas las notas registradas
                    if (matricula.NotaPrimerCorte > 0 && matricula.NotaSegundoCorte > 0 && matricula.NotaTercerCorte > 0)
                    {
                        sumaNotas += matricula.CalcularNotaFinal();
                        cantidadMaterias++;
                    }
                }

                return cantidadMaterias > 0 ? sumaNotas / cantidadMaterias : 0;
            }

            public Dictionary<string, List<string>> ListarMateriasAprobadasYReprobadas(int estudianteId, string periodo)
            {
                var matriculas = _matriculaRepository.GetByEstudianteYPeriodo(estudianteId, periodo);
                var resultado = new Dictionary<string, List<string>>
            {
                { "Aprobadas", new List<string>() },
                { "Reprobadas", new List<string>() },
                { "Cursando", new List<string>() }
            };

                foreach (var matricula in matriculas)
                {
                    string nombreCurso = matricula.Curso.Nombre;

                    if (matricula.Estado == EstadoMatricula.Aprobado)
                    {
                        resultado["Aprobadas"].Add($"{nombreCurso} - Nota: {matricula.CalcularNotaFinal():F2}");
                    }
                    else if (matricula.Estado == EstadoMatricula.Reprobado)
                    {
                        resultado["Reprobadas"].Add($"{nombreCurso} - Nota: {matricula.CalcularNotaFinal():F2}");
                    }
                    else
                    {
                        resultado["Cursando"].Add($"{nombreCurso} - {matricula.ObtenerEstadoActual()}");
                    }
                }

                return resultado;
            }

            public string GenerarBoletinEstudiante(int estudianteId, string periodo)
            {
                var estudiante = _estudianteRepository.GetById(estudianteId);
                if (estudiante == null)
                {
                    return "Error: El estudiante no existe";
                }

                var matriculas = _matriculaRepository.GetByEstudianteYPeriodo(estudianteId, periodo);
                if (!matriculas.Any())
                {
                    return $"El estudiante {estudiante.NombreCompleto} no tiene materias matriculadas en el periodo {periodo}";
                }

                double promedio = CalcularPromedioEstudiante(estudianteId, periodo);
                var materiasEstado = ListarMateriasAprobadasYReprobadas(estudianteId, periodo);

                string boletin = $"BOLETÍN DE CALIFICACIONES\n" +
                                $"Estudiante: {estudiante.NombreCompleto}\n" +
                                $"Código: {estudiante.Codigo}\n" +
                                $"Programa: {estudiante.Programa}\n" +
                                $"Semestre: {estudiante.Semestre}\n" +
                                $"Periodo: {periodo}\n\n" +
                                $"MATERIAS CURSADAS:\n";

                foreach (var matricula in matriculas)
                {
                    boletin += $"- {matricula.Curso.Nombre} ({matricula.Curso.Codigo})\n" +
                              $"  Primer corte (30%): {matricula.NotaPrimerCorte:F2}\n" +
                              $"  Segundo corte (30%): {matricula.NotaSegundoCorte:F2}\n" +
                              $"  Tercer corte (40%): {matricula.NotaTercerCorte:F2}\n" +
                              $"  Nota final: {matricula.CalcularNotaFinal():F2}\n" +
                              $"  Estado: {matricula.ObtenerEstadoActual()}\n\n";
                }

                boletin += $"PROMEDIO DEL PERIODO: {promedio:F2}\n\n";

                boletin += "MATERIAS APROBADAS:\n";
                foreach (var materia in materiasEstado["Aprobadas"])
                {
                    boletin += $"- {materia}\n";
                }

                boletin += "\nMATERIAS REPROBADAS:\n";
                foreach (var materia in materiasEstado["Reprobadas"])
                {
                    boletin += $"- {materia}\n";
                }

                boletin += "\nMATERIAS EN CURSO:\n";
                foreach (var materia in materiasEstado["Cursando"])
                {
                    boletin += $"- {materia}\n";
                }

                return boletin;
            }
        }
    }
}
}
