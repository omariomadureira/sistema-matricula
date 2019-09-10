using System.Web.Script.Serialization;

namespace SistemaMatricula.Helpers
{
    public class LogHelper
    {
        public static string Notes(object item, string error)
        {
            string notes = string.Empty;

            if (item != null)
            {
                string paramaters = new JavaScriptSerializer().Serialize(item);
                notes = string.Format("{0}", paramaters);
            }

            if (string.IsNullOrWhiteSpace(error) == false)
                notes = string.Format("{0}. Erro: {1}", notes, error);

            return notes;
        }

        public static string Notes(object[] itens, string error)
        {
            string notes = string.Empty;

            if (itens != null && itens.Length > 0)
            {
                notes = "Parâmetros: ";
                foreach (object item in itens)
                {
                    string paramaters = new JavaScriptSerializer().Serialize(item);
                    notes = string.Format("{0} {1}", notes, paramaters);
                }
            }

            if (string.IsNullOrWhiteSpace(error) == false)
                notes = string.Format("{0}. Erro: {1}", notes, error);

            return notes;
        }
    }
}