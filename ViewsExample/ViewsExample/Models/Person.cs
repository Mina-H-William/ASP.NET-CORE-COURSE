namespace ViewsExample.Models
{
    public class Person
    {
        public string? Name { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender gender { get; set; }

    }

    public enum Gender
    {
        Male, Female, Other
    }
}
