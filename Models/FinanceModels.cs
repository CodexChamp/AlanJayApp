using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlanJayApp.Models
{
    public class FinanceDeliveredReceivables
    {
        public int CompanyNumber { get; set; }
        public string? CompanyName { get; set; }
        public int? Units { get; set; }
        public double? TotalHours { get; set; }
        public decimal? Profit { get; set; }
    }

    public class FinanceUndeliveredReceivables
    {
        public int CompanyNumber { get; set; }
        public string? CompanyName { get; set; }
        public int? Units { get; set; }
        public double? TotalHours { get; set; }
        public decimal? Profit { get; set; }
    }

    public class FinanceMonthly
    {
        [Key]
        public int CompanyNumber { get; set; }
        public string? CompanyName { get; set; }
        public DateTime? VantageMonth { get; set; }
        public int? DeliveredCount { get; set; }
        public int? PaidCount { get; set; }
        public decimal? ProfitTotal { get; set; }
        public decimal? EstInterest { get; set; }
        public decimal? ActualInterestTotal { get; set; }
    }


    public class FinanceYearly
    {
        [Key]
        public int CompanyNumber { get; set; }
        public string? CompanyName { get; set; }
        public DateTime? VantageMonth { get; set; }
        public int? DeliveredCount { get; set; }
        public int? PaidCount { get; set; }
        public double? Days { get; set; }
        public decimal? ProfitTotal { get; set; }
        public decimal? EstInterest { get; set; }
        public decimal? ActualInterestTotal { get; set; }
    }

    public class FinancePayroll
    {
        [Key]
        public int PayrollID { get; set; }
        public decimal? MonthPayroll { get; set; }
        public decimal? YearPayroll { get; set; }
    }
    public class NotBooked
    {
        public int NotBookedCount { get; set; }
    }
}


