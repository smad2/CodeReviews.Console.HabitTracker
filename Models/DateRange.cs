public class DateRange
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; }

    public int TotalDays => (EndDate - StartDate).Days + 1;
}