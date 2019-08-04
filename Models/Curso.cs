using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class Curso
    {
        public Guid IdCurso { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Preencha um número válido")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public int Creditos { get; set; }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public static bool Incluir(Curso item)
        {
            return CursoDAO.Incluir(item);
        }

        public static Curso Consultar(Guid IdCurso)
        {
            return CursoDAO.Consultar(IdCurso);
        }

        public static List<Curso> Listar(string palavra = null)
        {
            return CursoDAO.Listar(palavra);
        }

        public static bool Alterar(Curso item)
        {
            return CursoDAO.Alterar(item);
        }

        public static bool Desativar(Guid IdCurso)
        {
            return CursoDAO.Desativar(IdCurso);
        }
    }
}