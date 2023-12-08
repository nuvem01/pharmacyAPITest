
namespace Nuvem.PharmacyManagement.PharmacyServices.Models;
public class PharmacistMTDReport
{   
    public int Pharmacyid {get; set;}
    public string? Pharmacy { get; set; }
    public int Pharmacistid {get; set;}
    public string? Pharmacist { get; set; }
    public string? DrugName { get; set; }
    public int UnitCount { get; set; }
    public decimal SaleAmount { get; set; }
}