namespace MudBlazorUIDemo.Flows
{
    public class RandomNameGenerator
    {
        string[] _names = new string[] { "Aaron", "Abdul", "Abe", "Abel", "Abraham", "Adam", "Adan", "Adolfo", "Adolph", "Adrian",
            "Abby", "Abigail", "Adele", "Adrian", "Bebe", "Cika", "Dora", "Eva", "Ferdi", "Gogi", "Holka", "Katty", "Lola", "Musa", "Ivan",
            "Nora"};

        string[] _lastNames = new string[] { "Abbott", "Acosta", "Adams", "Adkins", "Aguilar", "Jones", "Porsche", "Brisket", "Bond",
            "Kristy", "Rocky", "Pinola", "Maersk", "Zola", "Zhan", "Toumberg"};

        public string? FirstName {get;set;}
        public string? LastName {get;set;}
        public DateTime? DOB {get;set;}

        Random _rand = new Random((int)DateTime.Now.Ticks);

        public void Run()
        {
            FirstName = _names[_rand.Next(0, _names.Length - 1)];
            LastName = _lastNames[_rand.Next(0, _lastNames.Length - 1)];
            var y = 1900 + _rand.Next(0, 150);
            var m = _rand.Next(1, 12);
            var d = _rand.Next(0, 3);
            DOB = new DateTime(y, m, 1);
            DOB = DOB.Value.AddDays(d);
        }
    }
}
