using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.CapaEntidades;

namespace Entidades
{
    internal class Datos
    {
        using CapaEntidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CapaDatos
    {
        public class EstudianteRepository
        {
            private const string ARCHIVO_ESTUDIANTES = "Estudiantes.txt";
            private readonly string file;

            public EstudianteRepository()
            {
                file = ARCHIVO_ESTUDIANTES;
            }

            public List<Estudiante> Read()
            {
                try
                {
                    List<Estudiante> estudiantes = new List<Estudiante>();
                    if (!File.Exists(file)) return estudiantes;

                    StreamReader reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        estudiantes.Add(MappingType(reader.ReadLine()));
                    }
                    reader.Close();
                    return estudiantes;
                }
                catch (Exception)
                {
                    return new List<Estudiante>();
                }
            }

            public Estudiante MappingType(string datos)
            {
                string[] campos = datos.Split(';');
                Estudiante estudiante = new Estudiante
                {
                    Id = int.Parse(campos[0]),
                    Codigo = campos[1],
                    Nombre = campos[2],
                    Apellido = campos[3],
                    Email = campos[4],
                    Programa = campos[5],
                    Semestre = int.Parse(campos[6])
                };
                return estudiante;
            }

            public Estudiante GetById(int id)
            {
                return Read().FirstOrDefault(e => e.Id == id);
            }

            public Estudiante GetByCodigo(string codigo)
            {
                return Read().FirstOrDefault(e => e.Codigo == codigo);
            }

            public bool Save(Estudiante estudiante)
            {
                try
                {
                    List<Estudiante> estudiantes = Read();

                    // Si es nuevo, asignar ID
                    if (estudiante.Id == 0)
                    {
                        estudiante.Id = (estudiantes.Count > 0) ? estudiantes.Max(e => e.Id) + 1 : 1;
                        estudiantes.Add(estudiante);
                    }
                    else
                    {
                        // Si ya existe, actualizar
                        int index = estudiantes.FindIndex(e => e.Id == estudiante.Id);
                        if (index >= 0)
                        {
                            estudiantes[index] = estudiante;
                        }
                        else
                        {
                            estudiantes.Add(estudiante);
                        }
                    }

                    // Guardar todos
                    StreamWriter writer = new StreamWriter(file, false);
                    foreach (var e in estudiantes)
                    {
                        writer.WriteLine(e.ToString());
                    }
                    writer.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public class CursoRepository
        {
            private const string ARCHIVO_CURSOS = "Cursos.txt";
            private readonly string file;

            public CursoRepository()
            {
                file = ARCHIVO_CURSOS;
            }

            public List<Curso> Read()
            {
                try
                {
                    List<Curso> cursos = new List<Curso>();
                    if (!File.Exists(file)) return cursos;

                    StreamReader reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        cursos.Add(MappingType(reader.ReadLine()));
                    }
                    reader.Close();
                    return cursos;
                }
                catch (Exception)
                {
                    return new List<Curso>();
                }
            }

            public Curso MappingType(string datos)
            {
                string[] campos = datos.Split(';');
                Curso curso = new Curso
                {
                    Id = int.Parse(campos[0]),
                    Codigo = campos[1],
                    Nombre = campos[2],
                    Creditos = int.Parse(campos[3]),
                    NotaMinima = double.Parse(campos[4])
                };
                return curso;
            }

            public Curso GetById(int id)
            {
                return Read().FirstOrDefault(c => c.Id == id);
            }

            public Curso GetByCodigo(string codigo)
            {
                return Read().FirstOrDefault(c => c.Codigo == codigo);
            }

            public bool Save(Curso curso)
            {
                try
                {
                    List<Curso> cursos = Read();

                    // Si es nuevo, asignar ID
                    if (curso.Id == 0)
                    {
                        curso.Id = (cursos.Count > 0) ? cursos.Max(c => c.Id) + 1 : 1;
                        cursos.Add(curso);
                    }
                    else
                    {
                        // Si ya existe, actualizar
                        int index = cursos.FindIndex(c => c.Id == curso.Id);
                        if (index >= 0)
                        {
                            cursos[index] = curso;
                        }
                        else
                        {
                            cursos.Add(curso);
                        }
                    }

                    // Guardar todos
                    StreamWriter writer = new StreamWriter(file, false);
                    foreach (var c in cursos)
                    {
                        writer.WriteLine(c.ToString());
                    }
                    writer.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public class MatriculaRepository
        {
            private const string ARCHIVO_MATRICULAS = "Matriculas.txt";
            private readonly string file;
            private readonly EstudianteRepository _estudianteRepository;
            private readonly CursoRepository _cursoRepository;

            public MatriculaRepository()
            {
                file = ARCHIVO_MATRICULAS;
                _estudianteRepository = new EstudianteRepository();
                _cursoRepository = new CursoRepository();
            }

            public List<Matricula> Read()
            {
                try
                {
                    List<Matricula> matriculas = new List<Matricula>();
                    if (!File.Exists(file)) return matriculas;

                    StreamReader reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        matriculas.Add(MappingType(reader.ReadLine()));
                    }
                    reader.Close();
                    return matriculas;
                }
                catch (Exception)
                {
                    return new List<Matricula>();
                }
            }

            public Matricula MappingType(string datos)
            {
                string[] campos = datos.Split(';');
                Matricula matricula = new Matricula
                {
                    Id = int.Parse(campos[0]),
                    Estudiante = _estudianteRepository.GetById(int.Parse(campos[1])),
                    Curso = _cursoRepository.GetById(int.Parse(campos[2])),
                    Periodo = campos[3],
                    NotaPrimerCorte = double.Parse(campos[4]),
                    NotaSegundoCorte = double.Parse(campos[5]),
                    NotaTercerCorte = double.Parse(campos[6]),
                    Estado = (EstadoMatricula)int.Parse(campos[7])
                };
                return matricula;
            }

            public Matricula GetById(int id)
            {
                return Read().FirstOrDefault(m => m.Id == id);
            }

            public List<Matricula> GetByEstudiante(int estudianteId)
            {
                return Read().Where(m => m.Estudiante.Id == estudianteId).ToList();
            }

            public List<Matricula> GetByEstudianteYPeriodo(int estudianteId, string periodo)
            {
                return Read().Where(m => m.Estudiante.Id == estudianteId && m.Periodo == periodo).ToList();
            }

            public List<Matricula> GetByCurso(int cursoId)
            {
                return Read().Where(m => m.Curso.Id == cursoId).ToList();
            }

            public bool Save(Matricula matricula)
            {
                try
                {
                    List<Matricula> matriculas = Read();

                    // Si es nueva, asignar ID
                    if (matricula.Id == 0)
                    {
                        matricula.Id = (matriculas.Count > 0) ? matriculas.Max(m => m.Id) + 1 : 1;
                        matriculas.Add(matricula);
                    }
                    else
                    {
                        // Si ya existe, actualizar
                        int index = matriculas.FindIndex(m => m.Id == matricula.Id);
                        if (index >= 0)
                        {
                            matriculas[index] = matricula;
                        }
                        else
                        {
                            matriculas.Add(matricula);
                        }
                    }

                    // Guardar todas
                    StreamWriter writer = new StreamWriter(file, false);
                    foreach (var m in matriculas)
                    {
                        writer.WriteLine(m.ToString());
                    }
                    writer.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
}
