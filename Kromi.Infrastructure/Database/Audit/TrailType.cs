namespace Kromi.Infrastructure.Database.Audit
{
    public enum TrailType : byte
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }
}
