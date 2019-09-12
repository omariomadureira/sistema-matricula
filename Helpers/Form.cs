using System.ComponentModel.DataAnnotations;

namespace SistemaMatricula.Helpers
{
    public class Form
    {
        public static ValidationResult IsCPF(string cpf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    return new ValidationResult("Preenchimento obrigatório");

                cpf = cpf.Trim();
                cpf = cpf.Replace(".", "").Replace("-", "");

                if (cpf.Length != 11)
                    return new ValidationResult("Preencha um CPF válido");

                string[] blacklist =
                {
                    "00000", "11111", "22222", "33333", "44444", "55555",
                    "66666", "77777", "88888", "99999", "12345", "98765",
                    "56789", "54321"
                };

                foreach (string item in blacklist)
                {
                    if (cpf.Contains(item))
                        return new ValidationResult("Preencha um CPF válido");
                }

                string temp = cpf.Substring(0, 9);
                int sum = 0;
                int[] multiplier = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

                for (int i = 0; i < 9; i++)
                    sum += int.Parse(temp[i].ToString()) * multiplier[i];

                int rest = sum % 11;

                if (rest < 2)
                    rest = 0;
                else
                    rest = 11 - rest;

                string digit = rest.ToString();

                temp = temp + digit;
                sum = 0;
                multiplier = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

                for (int i = 0; i < 10; i++)
                    sum += int.Parse(temp[i].ToString()) * multiplier[i];

                rest = sum % 11;

                if (rest < 2)
                    rest = 0;
                else
                    rest = 11 - rest;

                digit = digit + rest.ToString();

                if (cpf.EndsWith(digit))
                    return ValidationResult.Success;
            }
            catch (System.Exception e)
            {
                string notes = LogHelper.Notes(cpf, e.Message);
                Models.Log.Add(Models.Log.TYPE_ERROR, "SistemaMatricula.Helpers.Form.IsCPF", notes);
            }

            return new ValidationResult("Preencha um CPF válido");
        }
    }
}