using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nuvem.PharmacyManagement.PharmacyServices.DatabaseContext;
using Nuvem.PharmacyManagement.PharmacyServices.DatabaseContext.EFEntities;
using Nuvem.PharmacyManagement.PharmacyServices.Models;

namespace Nuvem.PharmacyManagement.PharmacyServices;

public class PharmacyService : IPharmacyService
{
    private readonly IPharmacyDbContext _dbContext;
    private readonly AppSettingsConfiguraion _appConfig;
    private readonly ILogger _logger;
    public PharmacyService(IPharmacyDbContext dbContext, AppSettingsConfiguraion appConfig, ILogger<PharmacyService> logger)
    {

    }

    public async Task<PharmacistDisplayResult<PharmacistMTDReport>?> GetPharmacistListByPharmacyIdAsync(int pharmacyId,ParameterCollection parameters)
    {
        if(parameters.PageSize == 0) parameters.PageSize = 5;
        PharmacistDisplayResult<PharmacistMTDReport> result = new();
        var pharmacistReport =  await _dbContext.sp_PharmacistDrugMTDReport(pharmacyId);

        if(pharmacistReport is not null)
        {
        result.TotalCount = pharmacistReport.ToList().Count;
        int skip = parameters.Page * parameters.PageSize;
        var canPage = skip < pharmacistReport.ToList().Count;
        if(!canPage) return null;
        result.List = (IEnumerable<PharmacistMTDReport>?)pharmacistReport.Select(p=> p)
                        .Skip(skip)
                        .Take(parameters.PageSize);
        }

        return result;
    }
    public async Task<List<Pharmacy>> GetPharmacieListAsync()
    {      
        return await Task.FromResult( _dbContext.Pharmacy.ToList());
    }

    public async Task<Pharmacy?> GetPharmacyByIdAsync(int id)
    {
        return await _dbContext.Pharmacy.FirstOrDefaultAsync(x => x.PharmacyId == id);
    }

    public Task<Pharmacy?> UpdatePharmacyAsync(Pharmacy pharmacy)
    {
        var existingPharmacy = GetPharmacyByIdAsync(pharmacy.PharmacyId);
            if (existingPharmacy is null) 
            {
                _logger.LogInformation("Pharmacy to update not found!");
                return null;
            }
            pharmacy.UpdatedDate = DateTime.Now.ToString();

        try
        {
            _dbContext.Pharmacy.Attach(existingPharmacy.Result);
        }
        catch (Exception ex)
        {

        }
          

            var entry = _dbContext.Instance.Entry(existingPharmacy);
            entry.CurrentValues.SetValues(pharmacy);

            entry.Property("PharmacyId").IsModified = false;
            entry.Property("CreatedDate").IsModified = false;   

            _dbContext.SaveChangesAsync();  
            return existingPharmacy; 
    }   

    public PharmacyDisplayResult<Pharmacy> PaginatedPharmacyListAsync(ParameterCollection parameters)
    {
        if(parameters.PageSize == 0) parameters.PageSize = 5;
        PharmacyDisplayResult<Pharmacy> result = new();
        var pharmacyList = GetPharmacieListAsync();
        result.TotalCount = pharmacyList.Result.Count;
        int skip = parameters.Page * parameters.PageSize;
        var canPage = skip < pharmacyList.Result.Count;
        if(!canPage) return null;
        result.List = pharmacyList.Result.Select(p=> p)
                        .Skip(skip)
                        .Take(skip)
                        .ToList();

        return result;
    }

    public PharmacyDisplayResult<Pharmacy> PaginatedPharmacyListAsync2(ParameterCollection parameters, int test)
    {
        if (parameters.PageSize == 0) parameters.PageSize = 5;
        PharmacyDisplayResult<Pharmacy> result = new();
        var pharmacyList = GetPharmacieListAsync();
        result.TotalCount = pharmacyList.Result.Count;
        int skip = parameters.Page * parameters.PageSize;
        var canPage = skip < pharmacyList.Result.Count;
        if (!canPage) return null;
        result.List = pharmacyList.Result.Select(p => p)
                        .Skip(skip)
                        .Take(skip)
                        .ToList();

        return result;
    }
}