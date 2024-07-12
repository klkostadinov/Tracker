using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class SubscriptionViewModel
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}