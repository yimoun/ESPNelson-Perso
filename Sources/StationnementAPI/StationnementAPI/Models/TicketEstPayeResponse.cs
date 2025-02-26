namespace StationnementAPI.Models
{
    public class TicketEstPayeResponse
    {
        public string TicketId { get; set; } = string.Empty;
        public bool EstPaye { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? TempsArrivee { get; set; }
        public DateTime? TempsSortie { get; set; }
    }
}
