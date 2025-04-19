namespace Kromi.Domain.Entities.StoredProcedures
{
    public class GenericStringString
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public GenericStringString()
        {

        }
        public GenericStringString(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
