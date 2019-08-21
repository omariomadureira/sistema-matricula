﻿using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class DisciplinaSemestre
    {
        public Guid IdDisciplinaSemestre { get; set; }
        public Disciplina Disciplina { get; set; }
        public Semestre Semestre { get; set; }
        public Professor Professor { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string DiaSemana { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Horario { get; set; }
        public string Status { get; set; }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public static bool Incluir(DisciplinaSemestre item)
        {
            return DisciplinaSemestreDAO.Incluir(item);
        }

        public static DisciplinaSemestre Consultar(Guid IdDisciplinaSemestre)
        {
            return DisciplinaSemestreDAO.Consultar(IdDisciplinaSemestre);
        }

        public static List<DisciplinaSemestre> Listar(DisciplinaSemestre filtros)
        {
            return DisciplinaSemestreDAO.Listar(filtros);
        }

        public static object ListarGrade(Guid? IdSemestre = null, Guid? IdCurso = null, string StatusGrade = null, string PalavraChave = null)
        {
            return DisciplinaSemestreDAO.ListarGrade(IdSemestre, IdCurso, StatusGrade, PalavraChave);
        }

        public static bool Alterar(DisciplinaSemestre item)
        {
            return DisciplinaSemestreDAO.Alterar(item);
        }

        public static bool Desativar(Guid IdDisciplinaSemestre)
        {
            return DisciplinaSemestreDAO.Desativar(IdDisciplinaSemestre);
        }

        public static List<string> Dias()
        {
            return new List<string>()
            {
                "SEGUNDA-FEIRA",
                "TERÇA-FEIRA",
                "QUARTA-FEIRA",
                "QUINTA-FEIRA",
                "SEXTA-FEIRA",
                "SÁBADO",
                "DOMINGO"
            };
        }

        public static List<string> Horarios(string periodo)
        {
            switch (periodo)
            {
                case Semestre.PERIODO_MATUTINO:
                    return new List<string>()
                    {
                        "7:00",
                        "8:00",
                        "9:00",
                        "10:00",
                        "11:00",
                    };
                case Semestre.PERIODO_VESPERTINO:
                    return new List<string>()
                    {
                        "13:00",
                        "14:00",
                        "15:00",
                        "16:00",
                        "17:00",
                    };
                case Semestre.PERIODO_NOTURNO:
                    return new List<string>()
                    {
                        "19:00",
                        "20:00",
                        "21:00",
                        "22:00",
                        "23:00"
                    };
                default:
                    return new List<string>()
                    {
                        "7:00",
                        "8:00",
                        "9:00",
                        "10:00",
                        "11:00",
                        "13:00",
                        "14:00",
                        "15:00",
                        "16:00",
                        "17:00",
                        "19:00",
                        "20:00",
                        "21:00",
                        "22:00",
                        "23:00"
                    };
            }
        }

        public static List<string> StatusGrade()
        {
            return new List<string>()
            {
                "PENDENTE",
                "COMPLETO",
                "INCOMPLETO",
                "ERRO"
            };
        }
    }
}