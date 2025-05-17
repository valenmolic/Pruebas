using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    internal class entidades
    {
        using System;
using System.Collections.Generic;

namespace CapaEntidades
    {
        public class Estudiante
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Email { get; set; }
            public string Programa { get; set; }
            public int Semestre { get; set; }
            public List<Matricula> Matriculas { get; set; } = new List<Matricula>();

            public override string ToString()
            {
                return $"{Id};{Codigo};{Nombre};{Apellido};{Email};{Programa};{Semestre}";
            }

            public string NombreCompleto => $"{Nombre} {Apellido}";
        }

        public class Curso
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public int Creditos { get; set; }
            public double NotaMinima { get; set; } = 3.0;
            public List<Matricula> Matriculas { get; set; } = new List<Matricula>();

            public override string ToString()
            {
                return $"{Id};{Codigo};{Nombre};{Creditos};{NotaMinima}";
            }
        }

        public enum EstadoMatricula
        {
            Cursando,
            Aprobado,
            Reprobado
        }

        public class Matricula
        {
            public int Id { get; set; }
            public Estudiante Estudiante { get; set; }
            public Curso Curso { get; set; }
            public string Periodo { get; set; }
            public double NotaPrimerCorte { get; set; }
            public double NotaSegundoCorte { get; set; }
            public double NotaTercerCorte { get; set; }
            public EstadoMatricula Estado { get; set; } = EstadoMatricula.Cursando;

            // Porcentajes de evaluación
            public const double PORCENTAJE_PRIMER_CORTE = 0.30;
            public const double PORCENTAJE_SEGUNDO_CORTE = 0.30;
            public const double PORCENTAJE_TERCER_CORTE = 0.40;

            public override string ToString()
            {
                return $"{Id};{Estudiante.Id};{Curso.Id};{Periodo};{NotaPrimerCorte};{NotaSegundoCorte};{NotaTercerCorte};{(int)Estado}";
            }

            public double CalcularNotaFinal()
            {
                return (NotaPrimerCorte * PORCENTAJE_PRIMER_CORTE) +
                       (NotaSegundoCorte * PORCENTAJE_SEGUNDO_CORTE) +
                       (NotaTercerCorte * PORCENTAJE_TERCER_CORTE);
            }

            public bool EstaAprobado()
            {
                return CalcularNotaFinal() >= Curso.NotaMinima;
            }

            public string ObtenerEstadoActual()
            {
                if (Estado == EstadoMatricula.Cursando)
                {
                    double notaActual = CalcularNotaParcial();
                    double notaNecesaria = CalcularNotaNecesariaParaAprobar();

                    if (notaNecesaria > 5.0)
                        return $"Actualmente tiene {notaActual:F2}. Ya no puede aprobar la materia.";
                    else if (notaNecesaria <= 0)
                        return $"Actualmente tiene {notaActual:F2}. Ya aprobó la materia.";
                    else
                        return $"Actualmente tiene {notaActual:F2}. Necesita {notaNecesaria:F2} en el tercer corte para aprobar.";
                }
                else
                {
                    return Estado == EstadoMatricula.Aprobado ? "Aprobado" : "Reprobado";
                }
            }

            private double CalcularNotaParcial()
            {
                // Si no tiene tercer corte, calcula solo con los dos primeros
                if (NotaTercerCorte == 0)
                {
                    double porcentajeAcumulado = PORCENTAJE_PRIMER_CORTE + PORCENTAJE_SEGUNDO_CORTE;
                    return ((NotaPrimerCorte * PORCENTAJE_PRIMER_CORTE) +
                            (NotaSegundoCorte * PORCENTAJE_SEGUNDO_CORTE)) / porcentajeAcumulado;
                }
                return CalcularNotaFinal();
            }

            private double CalcularNotaNecesariaParaAprobar()
            {
                // Nota mínima para aprobar - lo que ya tiene de los dos primeros cortes, dividido por el peso del tercer corte
                double notaAcumulada = (NotaPrimerCorte * PORCENTAJE_PRIMER_CORTE) +
                                      (NotaSegundoCorte * PORCENTAJE_SEGUNDO_CORTE);
                double notaNecesaria = (Curso.NotaMinima - notaAcumulada) / PORCENTAJE_TERCER_CORTE;

                return Math.Max(0, Math.Min(5.0, notaNecesaria));
            }
        }
    }
}
}
