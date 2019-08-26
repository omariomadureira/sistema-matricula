﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<CursoData> CursoData { get; set; }
        public virtual DbSet<DisciplinaData> DisciplinaData { get; set; }
        public virtual DbSet<AlunoData> AlunoData { get; set; }
        public virtual DbSet<ProfessorData> ProfessorData { get; set; }
        public virtual DbSet<SemestreData> SemestreData { get; set; }
        public virtual DbSet<DisciplinaSemestreData> DisciplinaSemestreData { get; set; }
    
        public virtual ObjectResult<Grade_ListarCursos_Result> Grade_ListarCursos(Nullable<System.Guid> idSemestre, Nullable<System.Guid> idCurso, string statusGrade, string palavraChave)
        {
            var idSemestreParameter = idSemestre.HasValue ?
                new ObjectParameter("IdSemestre", idSemestre) :
                new ObjectParameter("IdSemestre", typeof(System.Guid));
    
            var idCursoParameter = idCurso.HasValue ?
                new ObjectParameter("IdCurso", idCurso) :
                new ObjectParameter("IdCurso", typeof(System.Guid));
    
            var statusGradeParameter = statusGrade != null ?
                new ObjectParameter("StatusGrade", statusGrade) :
                new ObjectParameter("StatusGrade", typeof(string));
    
            var palavraChaveParameter = palavraChave != null ?
                new ObjectParameter("PalavraChave", palavraChave) :
                new ObjectParameter("PalavraChave", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Grade_ListarCursos_Result>("Grade_ListarCursos", idSemestreParameter, idCursoParameter, statusGradeParameter, palavraChaveParameter);
        }
    
        public virtual ObjectResult<Grade_ListarDisciplinasParaMatricula_Result> Grade_ListarDisciplinasParaMatricula()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Grade_ListarDisciplinasParaMatricula_Result>("Grade_ListarDisciplinasParaMatricula");
        }
    }
}
