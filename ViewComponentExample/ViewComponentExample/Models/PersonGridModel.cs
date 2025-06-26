namespace ViewComponentExample.Models
{
    public class PersonGridModel
    {
        public string GridTitle { get; set; } = String.Empty;

        public List<Person> Persons { get; set; } = new List<Person>();
    }
}
