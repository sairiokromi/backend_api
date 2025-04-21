namespace Kromi.Application.Data.Models.Generic
{
    public class GenericIdName
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public GenericIdName()
        {

        }
        public GenericIdName(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
