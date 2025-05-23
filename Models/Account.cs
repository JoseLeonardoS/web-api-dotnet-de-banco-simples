﻿namespace WebBank.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public decimal Balance { get; set; } = 0m;
    }
}
