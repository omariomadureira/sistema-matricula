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
                return 2; //TODO: No final, alterar para 10.
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

        public int MaxPages
        {
            get
            {
                return 14;
            }
        }

        public decimal InitialPage
        {
            get
            {
                decimal initial = 1;

                if (MaxPages < Pages)
                {
                    decimal difference = Pages - Actual;

                    if (difference < MaxPages)
                    {
                        initial = Pages - MaxPages;
                    }
                    else
                    {
                        initial = Actual;
                    }
                }

                return initial;
            }
        }

        public decimal FinalPage
        {
            get
            {
                decimal final = Pages;

                if (MaxPages < Pages)
                {
                    decimal difference = Pages - Actual;

                    if (difference >= MaxPages)
                    {
                        final = Actual + MaxPages;
                    }
                }

                return final;
            }
        }
    }
}