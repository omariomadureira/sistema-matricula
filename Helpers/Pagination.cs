namespace SistemaMatricula.Helpers
{
    public class Pagination
    {
        public int Actual { get; set; } = 1;
        public int Rows { get; set; } = 0;

        public int ItensPerPage
        {
            get
            {
                return 5;
            }
        }

        public decimal Pages
        {
            get
            {
                if (Rows > 0)
                {
                    decimal result = (decimal)Rows / (decimal)ItensPerPage;
                    return System.Math.Ceiling(result);
                }

                return 1;
            }

        }
    }
}