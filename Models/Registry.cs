using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Models
{
    public class Registry
    {
        public Guid IdRegistry { get; set; }
        public Grid Grid { get; set; }
        public Student Student { get; set; }
        public bool? Alternative { get; set; }
        public DateTime RegisterDate { get; set; }
        public Guid RegisterBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }

        public static bool Add(Registry item)
        {
            return RegistryDAO.Add(item);
        }

        public static Registry Find(Guid id)
        {
            return RegistryDAO.Find(id);
        }

        public static List<Registry> List(Registry filters = null, bool active = false)
        {
            return RegistryDAO.List(filters, active);
        }

        public static List<Registry> GridList(Guid idStudent, Guid idCourse)
        {
            return RegistryDAO.GridList(idStudent, idCourse);
        }

        public static bool Delete(Guid id)
        {
            return RegistryDAO.Delete(id);
        }

        public static bool DeleteByGrid(Guid id)
        {
            return RegistryDAO.DeleteByGrid(id);
        }

        public static ValidationResult Allow(Controllers.RegistryView[] list)
        {
            if (list == null)
                return new ValidationResult("Não foi possível realizar a matrícula. Tente novamente mais tarde.");

            var student = Student.FindLoggedUser();

            if (student == null)
                return new ValidationResult("Não foi possível realizar a matrícula. Tente novamente mais tarde.");

            Registry filters = new Registry()
            {
                Student = student
            };

            var registries = List(filters, true);

            int restForFirst = 4 - registries.FindAll(x => !x.Alternative.HasValue || !x.Alternative.Value).Count;
            int restForSecond = 2 - registries.FindAll(x => x.Alternative.HasValue && x.Alternative.Value).Count;

            int first = 0, second = 0;

            foreach (Controllers.RegistryView item in list)
            {
                if (item.FirstOption == true)
                    first++;
                else if (item.SecondOption == true)
                    second++;
            }

            if (restForFirst < 1 && restForSecond < 1)
                return new ValidationResult("A quantidade máxima de matrículas foi atingida.");

            if (restForFirst > 0 && first == 0)
                return new ValidationResult("Escolha pelo menos 1 disciplina para primeira opção.");

            if (restForFirst > 0 && first > restForFirst)
                return new ValidationResult(
                    string.Format("Escolha no máximo {0} disciplina(s) para primeira opção.", restForFirst));

            if (restForFirst == 0 && first > restForFirst)
                return new ValidationResult("A quantidade máxima de matrículas para primeira opção foi atingida.");

            if (restForSecond > 0 && second == 0)
                return new ValidationResult("Escolha pelo menos 1 disciplina para segunda opção.");

            if (restForSecond > 0 && second > restForSecond)
                return new ValidationResult(
                    string.Format("Escolha no máximo {0} disciplina(s) para segunda opção.", restForSecond));

            return ValidationResult.Success;
        }

        public static void AcionarSistemaCobranca()
        {
            //TODO: Simular acionamento do sistema de cobrança
        }
    }
}