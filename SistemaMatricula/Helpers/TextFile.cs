namespace SistemaMatricula.Helpers
{
    public class TextFile
    {
        public string Name { get; set; }
        public string Location { get; set; }

        public TextFile(string Name)
        {
            this.Name = Name;
            Location = @"C:\Users\User\source\repos\SistemaMatricula\SistemaMatricula\Database\Files\{0}.txt";
        }

        public void Add(string[] lines)
        {
            System.IO.File.AppendAllLines(string.Format(Location, Name), lines);
        }

        public void Update(string[] lines)
        {
            System.IO.File.WriteAllLines(string.Format(Location, Name), lines);
        }

        public string[] Read()
        {
            return System.IO.File.ReadAllLines(string.Format(Location, Name));
        }
    }
}