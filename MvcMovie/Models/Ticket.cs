namespace MvcMovie.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string? TicketType { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public long Quantity { get; set; }
    }
}
