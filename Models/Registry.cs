﻿using SistemaMatricula.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaMatricula.Helpers;

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

            if (registries == null)
                return new ValidationResult("Não foi possível realizar a matrícula. Tente novamente mais tarde.");

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

        public static List<Registry> IsFull(List<Registry> itens)
        {
            try
            {
                if (itens == null)
                    return null;

                List<Grid> list = new List<Grid>();

                foreach (Registry item in itens)
                    list.Add(item.Grid);

                foreach (Grid item in list)
                {
                    if (item.Registries > 9)
                    {
                        item.Status = Grid.FINISHED;

                        var update = Grid.Update(item);

                        if (update == false)
                            throw new Exception("Grade não atualizada");

                        itens.RemoveAll(x => x.Grid.IdGrid == item.IdGrid);
                    }
                }

                return itens;
            }
            catch (Exception e)
            {
                string notes = LogHelper.Notes(itens, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Models.Registry.IsFull", notes);
            }

            return null;
        }

        public static List<Registry> GridList(Guid idStudent, Guid idCourse)
        {
            var list = RegistryDAO.GridList(idStudent, idCourse);

            if (list == null)
                return null;

            list = IsFull(list);

            if (list == null)
                throw new Exception("Erro na checagem da grade com quantidade máxima de matrículas");

            return list;
        }

        public static void AcionarSistemaCobranca()
        {
            //TODO: Simular acionamento do sistema de cobrança
        }
    }
}