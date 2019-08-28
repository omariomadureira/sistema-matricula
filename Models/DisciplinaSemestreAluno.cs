﻿using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class DisciplinaSemestreAluno
    {
        public DisciplinaSemestre DisciplinaSemestre { get; set; }
        public Aluno Aluno { get; set; }
        public bool? Alternativa { get; set; }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public static bool Incluir(DisciplinaSemestreAluno item)
        {
            return DisciplinaSemestreAlunoDAO.Incluir(item);
        }

        public static DisciplinaSemestreAluno Consultar(DisciplinaSemestreAluno item)
        {
            return DisciplinaSemestreAlunoDAO.Consultar(item);
        }

        public static List<DisciplinaSemestreAluno> Listar(DisciplinaSemestreAluno filtro = null)
        {
            return DisciplinaSemestreAlunoDAO.Listar(filtro);
        }

        public static List<DisciplinaSemestreAluno> ListarGrade(Guid IdCurso)
        {
            return DisciplinaSemestreAlunoDAO.ListarGrade(IdCurso);
        }

        public static bool Alterar(DisciplinaSemestreAluno item)
        {
            return DisciplinaSemestreAlunoDAO.Alterar(item);
        }

        public static bool Desativar(DisciplinaSemestreAluno item)
        {
            return DisciplinaSemestreAlunoDAO.Desativar(item);
        }

        public static void AcionarSistemaCobranca()
        {

        }

        public static void PermitirMatricula()
        {

        }
    }
}