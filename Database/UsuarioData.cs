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
    
    public partial class UsuarioData
    {
        public System.Guid IdUsuario { get; set; }
        public string Email { get; set; }
        public byte[] Senha { get; set; }
        public string Perfil { get; set; }
        public System.DateTime CadastroData { get; set; }
        public System.Guid CadastroPor { get; set; }
        public Nullable<System.DateTime> ExclusaoData { get; set; }
        public Nullable<System.Guid> ExclusaoPor { get; set; }
    }
}