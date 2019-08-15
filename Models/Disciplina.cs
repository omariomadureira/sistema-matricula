using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class Disciplina
    {
        public Guid IdDisciplina { get; set; }
        public Curso Curso { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(100, ErrorMessage = "O campo deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public static bool Incluir(Disciplina item)
        {
            return DisciplinaDAO.Incluir(item);
        }

        public static Disciplina Consultar(Guid IdDisciplina)
        {
            return DisciplinaDAO.Consultar(IdDisciplina);
        }

        public static List<Disciplina> Listar(Disciplina filtros)
        {
            return DisciplinaDAO.Listar(filtros);
        }

        public static bool Alterar(Disciplina item)
        {
            return DisciplinaDAO.Alterar(item);
        }

        public static bool Desativar(Guid IdDisciplina)
        {
            return DisciplinaDAO.Desativar(IdDisciplina);
        }
    }
}