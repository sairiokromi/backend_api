namespace Kromi.Domain.Entities.Contract
{
    public interface ITimeStamp
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
