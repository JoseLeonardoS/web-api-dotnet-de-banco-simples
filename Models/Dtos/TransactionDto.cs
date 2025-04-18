namespace WebBank.Models.Dtos
{
    public class TransactionDto
    {
        public int UserId { get; set; }
        public decimal Value { get; set; } = 0m;
    }
}
