//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SistemaMatricula.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProfessorData
    {
        public System.Guid IdProfessor { get; set; }
        public string Nome { get; set; }
        public System.DateTime DataNascimento { get; set; }
        public string Email { get; set; }
        public int CPF { get; set; }
        public string Curriculo { get; set; }
        public System.Guid CadastroPor { get; set; }
        public Nullable<System.Guid> ExclusaoPor { get; set; }
        public System.DateTime CadastroData { get; set; }
        public Nullable<System.DateTime> ExclusaoData { get; set; }
    }
}
