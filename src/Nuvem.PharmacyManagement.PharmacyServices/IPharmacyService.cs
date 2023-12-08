using Nuvem.PharmacyManagement.PharmacyServices.DatabaseContext.EFEntities;
using Nuvem.PharmacyManagement.PharmacyServices.Models;

namespace Nuvem.PharmacyManagement.PharmacyServices;
public interface IPharmacyService
{
    Task<List<Pharmacy>> GetPharmacieListAsync();
    Task<Pharmacy?> GetPharmacyByIdAsync(int id);
    Task<Pharmacy?> UpdatePharmacyAsync(Pharmacy pharmacy);
    PharmacyDisplayResult<Pharmacy> PaginatedPharmacyListAsync(ParameterCollection parameters);
    Task<PharmacistDisplayResult<PharmacistMTDReport>?> GetPharmacistListByPharmacyIdAsync(int pharmacyId,ParameterCollection parameters);
}
