using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class Semestre
    {
        public Guid IdSemestre { get; set; }
        public string Periodo { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime InicioData { get; set; }
        public DateTime CadastroData { get; set; }
        public Guid CadastroPor { get; set; }
        public DateTime? ExclusaoData { get; set; }
        public Guid? ExclusaoPor { get; set; }

        public static bool Incluir(Semestre item)
        {
            return SemestreDAO.Incluir(item);
        }

        public static Semestre Consultar(Guid IdSemestre)
        {
            return SemestreDAO.Consultar(IdSemestre);
        }

        public static List<Semestre> Listar(string palavra = null)
        {
            return SemestreDAO.Listar(palavra);
        }

        public static bool Alterar(Semestre item)
        {
            return SemestreDAO.Alterar(item);
        }

        public static bool Desativar(Guid IdSemestre)
        {
            return SemestreDAO.Desativar(IdSemestre);
        }

        public static List<string> Periodos()
        {
            return new List<string>()
            {
                "MATUTINO",
                "VESPERTINO",
                "NOTURNO"
            };
        }
    }
}