using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public double ElectricityForBefore { get; set; }
        public double WaterUsageForBefore { get; set; }
        public double ElectricityUsage { get; set; }
        public double WaterUsage { get; set; }
        public decimal ServiceFee { get; set; }
        public string? ServiceDetails { get; set; }
        public decimal AdditionalServiceFee { get; set; }
        public decimal TotalMoney { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? QRCodeImage { get; set; }
        public string StatusInvocie { get; set; }
        public string RentalContractId { get; set; }
        public string RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; }
    }
}
