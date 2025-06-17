using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AlanJayApp.Services; // ✅ Import the DatabaseConnectionService namespace


namespace AlanJayApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DatabaseConnectionService _dbConnectionService;

        public ApplicationDbContext(DatabaseConnectionService dbConnectionService)
        {
            _dbConnectionService = dbConnectionService;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _dbConnectionService.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }

        // Define the DbSets for each entity/table in the database
        public DbSet<FinanceDeliveredReceivables> FinanceDeliveredReceivables { get; set; } = default!;
        public DbSet<FinanceMonthly> FinanceMonthly { get; set; } = default!;
        public DbSet<FinancePayroll> FinancePayroll { get; set; } = default!;
        public DbSet<FinanceUndeliveredReceivables> FinanceUndeliveredReceivables { get; set; } = default!;
        public DbSet<FinanceYearly> FinanceYearly { get; set; } = default!;
        public DbSet<UserWebGroup> UserWebGroups { get; set; } = default!;

        public DbSet<VehiclesScanned> VehiclesScanned { get; set; } = default!;
        public DbSet<AzQuoteID> AzQuoteIDs { get; set; } = default!;
        public DbSet<AzQuoteOptions> AzQuoteOptions { get; set; } = default!;
        public DbSet<AzOptions> AzOptions { get; set; } = default!;
        public DbSet<AzVehicles> AzVehicles { get; set; } = default!;
        public DbSet<WindowSticker> WindowStickers { get; set; } = default!;
        public DbSet<WindowStickerOption> WindowStickerOptions { get; set; }
        public DbSet<AzQuoteSummary> AzQuoteSummary { get; set; }
        public DbSet<AzAgency> AzAgency { get; set; }
        public DbSet<AzQCitems> AzQCitems { get; set; }
        public DbSet<AzEmployees> AzEmployees { get; set; }
        public DbSet<AzEmployeeWebGroup> AzEmployeeWebGroups { get; set; }
        public DbSet<AzVehicleDeal> AzVehicleDeals { get; set; }

        public DbSet<AzVendors> AzVendors { get; set; }
        public DbSet<AzPOs> AzPOs { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FinanceDeliveredReceivables>().ToTable("financeDeliveredReceivables");
            modelBuilder.Entity<FinanceMonthly>().ToTable("financeMonthly");
            modelBuilder.Entity<FinancePayroll>().ToTable("financePayroll");
            modelBuilder.Entity<FinanceUndeliveredReceivables>().ToTable("financeUndeliveredReceivables");
            modelBuilder.Entity<FinanceYearly>().ToTable("financeYearly");
            modelBuilder.Entity<UserWebGroup>().ToTable("UserWebGroups");
            modelBuilder.Entity<AzEmployees>().ToTable("azEmployees");
            modelBuilder.Entity<AzEmployeeWebGroup>().ToTable("azEmployeeWebGroup");
            modelBuilder.Entity<VehiclesScanned>().ToTable("VehiclesScanned");
            modelBuilder.Entity<AzQuoteID>().ToTable("azQuoteID");
            modelBuilder.Entity<AzQuoteOptions>().ToTable("azQuoteOptions");
            modelBuilder.Entity<AzOptions>().ToTable("azOptions");
            modelBuilder.Entity<AzVehicles>().ToTable("azVehicles");
            // Map WindowSticker to existing table
            modelBuilder.Entity<WindowSticker>().ToTable("WindowSticker");

            // Map WindowStickerOptions for tracking incomplete options
            modelBuilder.Entity<WindowStickerOption>().ToTable("WindowStickerOptions");
            modelBuilder.Entity<AzVendors>().ToTable("azVendors");
            modelBuilder.Entity<AzPOs>().ToTable("azPOs");
            modelBuilder.Entity<AzQuoteSummary>().ToTable("azQuoteSummary");
            modelBuilder.Entity<AzAgency>().ToTable("azAgency");
            modelBuilder.Entity<AzQCitems>().ToTable("azQCitems");
            modelBuilder.Entity<AzVehicleDeal>().ToTable("azVehicleDeal");

            base.OnModelCreating(modelBuilder);
        }

    }

    // Define the Employee model
    public class UserWebGroup
    {
        [Key]
        public int EmployeeID { get; set; }
        public string LoginID { get; set; } = string.Empty;
        public string? Password { get; set; }
        public bool webGroup1 { get; set; }
        public bool webGroup2 { get; set; }
        public bool webGroup3 { get; set; }
        public bool webGroup4 { get; set; }
        public bool webGroup5 { get; set; }
    }
    public class AzEmployees
    {
        [Key]
        public int EmployeeID { get; set; }

        public string? Employee { get; set; } // Mapped to Employee
        public int? JobTitleID { get; set; }
        public string? Email { get; set; }
        public string? TollFree { get; set; }
        public string? DirectLine { get; set; }
        public string? CellPhone { get; set; }
        public int? SortOrder { get; set; }

        public string? LoginID { get; set; }
        public string? Password { get; set; }

        public int? AccessLevel { get; set; }
        public decimal? WeeklyDraw { get; set; }
        public decimal? DeliveryCharge { get; set; }
        public decimal? Salary { get; set; }
        public int? CommissionType { get; set; }
        public int? CommissionPercent { get; set; }
        public string? EmployeeNumber { get; set; }
        public int? SecurityGroup { get; set; }
        public int? Quotes { get; set; }
        public int? Lists { get; set; }
        public int? Service { get; set; }
        public int? Settings { get; set; }
        public int? Cost { get; set; }

        public bool? Group0 { get; set; }
        public bool? Group1 { get; set; }
        public bool? Group2 { get; set; }
        public bool? Group3 { get; set; }
        public bool? Group4 { get; set; }

        public bool? Individual0 { get; set; }
        public bool? Individual1 { get; set; }
        public bool? Individual2 { get; set; }
        public bool? Individual3 { get; set; }
        public bool? Individual4 { get; set; }
        public bool? Individual5 { get; set; }

        public bool? Emailer { get; set; }
        public bool? Inactive { get; set; }
    }


    // Define the FinanceDeliveredReceivables model
    public class FinanceDeliveredReceivables
    {
        [Key]
        public int CompanyNumber { get; set; }
        public string? CompanyName { get; set; }
        public decimal? Profit { get; set; }
        public int? Units { get; set; }
        public double? DaysAge { get; set; }
        public double? TotalHours { get; set; }
    }

    // Define the FinanceMonthly model
    public class FinanceMonthly
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

    // Define the FinancePayroll model
    public class FinancePayroll
    {
        [Key]
        public int PayrollID { get; set; }
        public decimal? MonthPayroll { get; set; }
        public decimal? YearPayroll { get; set; }
    }

    // Define the FinanceTotals model


    // Define the FinanceUndeliveredReceivables model
    public class FinanceUndeliveredReceivables
    {
        [Key]
        public int CompanyNumber { get; set; }
        public string? CompanyName { get; set; }
        public decimal? Profit { get; set; }
        public int? Units { get; set; }
        public double? DaysAge { get; set; }
        public double? TotalHours { get; set; }
    }

    // Define the FinanceYearly model
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


    public class VehiclesScanned
    {
        [Key]
        public string VIN { get; set; } = string.Empty;
        public int vehicleID { get; set; }
        public bool damagedInTransit { get; set; }
        public string? location { get; set; }

        public string? note { get; set; }
        public string? KeyCode { get; set; }

        public bool locationChanged { get; set; }
        public bool keyCodeChanged { get; set; }
        public bool OnHold { get; set; }
        public string? GoingTo { get; set; }
        public bool OnHoldChanged { get; set; }
    }
    public class AzVehicles
    {
        [Key]
        public int VehicleID { get; set; }
        public int? QuoteID { get; set; }
        public string? VIN { get; set; }
        public string? StockID { get; set; }
        public string? OrderNumber { get; set; }
        public string? Color { get; set; }
        public string? KeyCode { get; set; }
        public string? Vyear { get; set; }
        public string? Vmake { get; set; }
        public string? Vmodel { get; set; }
        public string? Vbody { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? Ordered { get; set; }
        public DateTime? MSOarrived { get; set; }
        public DateTime? InvoiceETA { get; set; }
        public DateTime? Arrived { get; set; }
        public DateTime? PartsArrived { get; set; }
        public DateTime? PartsLoaded { get; set; }
        public DateTime? Ready { get; set; }
        public DateTime? Delivered { get; set; }
        public DateTime? Contacted { get; set; }
        public DateTime? Paid { get; set; }
        public DateTime? PaidOff { get; set; }
        public DateTime? Funded { get; set; }
        public int? DaysInTransit { get; set; }
        public string? ModDesc { get; set; }
        public string? CAR { get; set; }
        public DateTime? DlvPDF { get; set; }
        public DateTime? InvPDF { get; set; }
        public DateTime? DppPDF { get; set; }
        public DateTime? CostPDF { get; set; }
        public string? IncentiveRptNote { get; set; }
        public string? MilestoneNote { get; set; }
        public string? DealerTrade { get; set; }
        public string? AssetNumber { get; set; }
        public DateTime? NewETA { get; set; }
        public DateTime? DispatchDate { get; set; }
        public int? DispatchEmployee { get; set; }
        public bool? Financed { get; set; }
        public int? ShopHrsWorked { get; set; }
        public string? HrsCompleted { get; set; }
        public string? Location { get; set; }
        public DateTime? LocationDate { get; set; }
        public string? RemQuarterlyRpt { get; set; }
        public int? ChaseAssignmentID { get; set; }
        public string? PickUpNote { get; set; }
        public bool? PayUponArrival { get; set; }
        public string? Retail { get; set; }
        public bool? CommissionDelivery { get; set; }
        public int? DmAddPOID { get; set; }
        public DateTime? TagPrint { get; set; }
        public bool? OnHold { get; set; }
        public string? GoingTo { get; set; }
        public string? QtrVehicleOption { get; set; }
        public string? Parts { get; set; }
        public bool? WatchFlag { get; set; }
        public int? FOVPStatus { get; set; }
        public string? FOVPConfirm { get; set; }
        public string? FOVPNote { get; set; }
        public int? DeliveryNote { get; set; }
        public int? RecallNote { get; set; }
        public DateTime? CorpDeliveries { get; set; }
        public string? InvoiceNo { get; set; }
        public string? ROno { get; set; }
        public string? RecievableRptNote { get; set; }
        public bool? Recall { get; set; }

        [ForeignKey("QuoteID")]
        public AzQuoteID? Quote { get; set; }
    }
    public class AzOptions
    {
        [Key]
        public int OptionID { get; set; }
        public string? OptionName { get; set; }
        public string? OptionDescription { get; set; }
        public int? YearID { get; set; }
        public int? BunkerID { get; set; }
        public bool? Original { get; set; }
        public int? OptionType { get; set; }
        public decimal? DefaultPrice { get; set; }
        public int? DefaultEqID { get; set; }
    }
    public class AzQuoteID
    {
        [Key]
        public int Quote_ID { get; set; }
        public int Version { get; set; }
        public int? Quoted_By { get; set; }
        public DateTime? Quote_Date { get; set; }
        public DateTime? QuotePrintDate { get; set; }
        public string? FileName { get; set; }
        public string? FileLocation { get; set; }
        public decimal? Amount { get; set; }
        public bool Active { get; set; }

        public ICollection<AzQuoteOptions> QuoteOptions { get; set; } = new List<AzQuoteOptions>();
    }
    public class AzQuoteOptions
    {
        [Key]
        public int ID { get; set; }
        public int QuoteID { get; set; }
        public int QVersion { get; set; }
        public int OptionID { get; set; }
        public int? EqListID { get; set; }
        public decimal? Price { get; set; }
        public int? Sort { get; set; }
        public string? Invoice { get; set; }
        public int? DetailType { get; set; }
        public bool? EstimatedTTT { get; set; }

        [ForeignKey("QuoteID")]
        public AzQuoteID Quote { get; set; } = default!;

        [ForeignKey("OptionID")]
        public AzOptions Option { get; set; } = default!;
    }

    public class WindowSticker
    {
        [Key]
        public int VehicleID { get; set; }
        public string? Inspector { get; set; }
        public DateTime? Date { get; set; }
        public bool Status { get; set; } // True: Ready, False: Not Ready
        public bool Damaged { get; set; }
        public string? Note { get; set; }
    }

    public class WindowStickerOption
    {
        [Key]
        public int Id { get; set; }
        public int VehicleID { get; set; }
        public int OptionID { get; set; }
        public bool isCompleted { get; set; }
    }

    public class AzQuoteSummary
    {
        [Key]
        public int QuoteID { get; set; }

        public decimal? BasePrice { get; set; }
        public DateTime? OriginalDate { get; set; }
        public string Specification { get; set; } = string.Empty;
        public string PageLine { get; set; } = string.Empty;
        public decimal? MSRP { get; set; }
        public int? CompanyNumber { get; set; }
        public string DropShipCode { get; set; } = string.Empty;
        public string FEID { get; set; } = string.Empty;
        public decimal? ContractFee { get; set; }
        public decimal? HybContractFee { get; set; }
        public string DelShipCode { get; set; } = string.Empty;
        public DateTime? PORecieveDate { get; set; }
        public decimal? TireBattery { get; set; }
        public decimal? StateTax { get; set; }
        public decimal? CountyTax { get; set; }
        public string Quote_Type { get; set; } = string.Empty;
        public int? Agency_Contact { get; set; }
        public int? shipID { get; set; }
        public string ActualModel { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string ModelDescription { get; set; } = string.Empty;
        public string bedLength { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string ContractNumber { get; set; } = string.Empty;
        public string POnumber { get; set; } = string.Empty;
        public string FAN { get; set; } = string.Empty;
        public string FAN2 { get; set; } = string.Empty;
        public string bidRefID { get; set; } = string.Empty;
        public string CustAsset { get; set; } = string.Empty;
        public bool? Stock { get; set; }
        public int? FromStock { get; set; }
        public bool? UsedVehicle { get; set; }
        public bool? ArchiveDelete { get; set; }
        public int? BunkerVID { get; set; }
        public bool? Parts { get; set; }
        public bool? LOI { get; set; }
        public bool? FOVPsent { get; set; }
        public DateTime? ACKSent { get; set; }
        public int? vehicleAcctID { get; set; }
        public decimal? bunMSRP { get; set; }
        public decimal? bunDealerNet { get; set; }
        public decimal? bunDFC { get; set; }
        public decimal? bunSubTotal { get; set; }
        public decimal? bunContractPrice { get; set; }
        public string bunVehicleSubGroup { get; set; } = string.Empty;
        public string bunVehicleGroup { get; set; } = string.Empty;
        public decimal? bunGPC { get; set; }
        public decimal? bunAssurance { get; set; }
        public string bunYear { get; set; } = string.Empty;
        public string bunManufacturer { get; set; } = string.Empty;
        public decimal? bunPercent { get; set; }
        public string partsNotes { get; set; } = string.Empty;
    }

    public class AzAgency
    {
        [Key]
        public int AgencyContactID { get; set; }

        public string Agency { get; set; } = string.Empty;
        public string ProperName { get; set; } = string.Empty;
        public string DAN { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Cell { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool? Inactive { get; set; }
        public string QuoteType { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
        public int? DefaultShip { get; set; }
    }

    public class AzQCitems
    {

        [Key]
        public int Id { get; set; }
        public string QCitem { get; set; } = string.Empty;
        public int Sort { get; set; }


    }

    public class AzEmployeeWebGroup
    {
        [Key]
        public int EmployeeID { get; set; } // Foreign Key

        public bool? webGroup1 { get; set; }
        public bool? webGroup2 { get; set; }
        public bool? webGroup3 { get; set; }
        public bool? webGroup4 { get; set; }
        public bool? webGroup5 { get; set; }
        public bool? webGroup6 { get; set; }
        public bool? webGroup7 { get; set; }

        [ForeignKey("EmployeeID")]
        public AzEmployees? Employee { get; set; } // Navigation Property
    }
    public class AzVehicleDeal
    {
        [Key]
        public int VehicleID { get; set; }

        public string? DealNo { get; set; }

        public DateTime? IntCommDate { get; set; }

        public string? InvAmtAccount { get; set; }

        public decimal? InvoiceAmount { get; set; }

        public string? SubTotAccount { get; set; }

        public decimal? SubTotAmount { get; set; }

        public decimal? UpfitInterest { get; set; }

        public decimal? Interest { get; set; }

        public string? Note { get; set; }

        public decimal? WeOwe { get; set; }

        public decimal? AddCost { get; set; }

        public decimal? NetProfit { get; set; }

        public string? Cash { get; set; }

        public decimal? FinanceReserve { get; set; }
    }
    public class AzVendors
    {
        [Key]
        public int VendorID { get; set; }

        [Required, StringLength(255)]
        public string Vendor { get; set; } = null!;

        public int DefaultMarkUp { get; set; }

        [StringLength(255)]
        public string? Make { get; set; }

        [StringLength(255)]
        public string? DropShipCode { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? Phone { get; set; }

        [StringLength(255)]
        public string? Fax { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Other { get; set; }

        [StringLength(255)]
        public string? Location { get; set; }

        [StringLength(255)]
        public string? VendorNumber { get; set; }

        public bool Inactive { get; set; }
    }
    public class AzPOs
    {
        [Key]
        public int POID { get; set; }

        public int? QuoteID { get; set; }
        public int? VendorID { get; set; }
        public int? ShipID { get; set; }
        public bool? ShipToAgency { get; set; }
        public int? PONum { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Total { get; set; }

        public DateTime? PrintDate { get; set; }
        public bool? Revision { get; set; }
        public int? Hours { get; set; }
        public bool? PartsRec { get; set; }
        public DateTime? PartsRecDate { get; set; }
        public bool? ServiceComplete { get; set; }
    }
}
