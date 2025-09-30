using FuzzySharp;
using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;
using System.Linq;
namespace Loujico.BL

{
    public interface ICompanys
    {
        public Task<object> GetAll(int page, int count, int? legalFilter);
        public Task<List<object>> GetAllCustomersIdAndName();
        public Task<object> GetById(int id);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count);
        public Task<bool> Edit(Co_Company_Name Company);
        public Task<bool> Add(Co_Company_Name Company);
        public Task<bool> Delete(int id);
        public Task<int> Count(string? legalFilter);
        public Task<List<Co_Company_Name>> Search(string name, int page, int count);
    }
    public class ClsCompany : ICompanys
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;
        IHistory ClsHistory;

        public ClsCompany(CompanySystemContext companySystemContext, Ilog clsLogs, IHistory clsHistory)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
            ClsHistory = clsHistory;
        }

        public async Task<object> GetAll(int page, int count, int? legalFilter)
        {
            try
            {
                var query = CTX.Co_Companies
                    .AsNoTracking()
                    .Include(c => c.Addresses)
                    .Include(c => c.Contacts)
                    .Include(c => c.CompanyActivities)
                        .ThenInclude(ca => ca.Activity)
                    .Include(c => c.Legal)
                    .Where(x => !x.IsDeleted);

                // تطبيق الفلترة على الـ Legal إذا كانت موجودة
                if (legalFilter>1)
                {
                    query = query.Where(c => c.LegalId== legalFilter);
                }

                var result = await query
                    .Skip((page - 1) * count)
                    .Take(count)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Comm_No,
                        c.Tax_No,
                        c.Found_Date,
                        c.CompanyDescription,
                        c.CreatedAt,
                        c.UpdatedAt,
                        c.LastVisit,
                        c.CreatedBy,
                        c.UpdatedBy,
                        Legal = c.Legal == null
                            ? null
                            : new { c.Legal.Id, c.Legal.LegalInfo },

                        Addresses = c.Addresses.Select(a => new
                        {
                            a.Id,
                            a.CountryId,
                            a.StateId,
                            a.CityId,
                            a.AddressLine
                        }),

                        Contacts = c.Contacts.Select(ct => new
                        {
                            ct.Id,
                            ct.ContactTypeId,
                            ct.Name
                        }),

                        Activities = c.CompanyActivities.Select(ca => new
                        {
                            ca.ActivityId,
                            ca.Activity.Name,
                            ca.Activity.IndustryId
                        })
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<object> GetById(int id)
        {
            try
            {
                var result = await CTX.Co_Companies
                    .AsNoTracking()
                    .Where(c => c.Id == id && !c.IsDeleted)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Comm_No,
                        c.Tax_No,
                        c.Found_Date,
                        c.CompanyDescription,
                        c.CreatedAt,
                        c.UpdatedAt,
                        c.LastVisit,
                        c.CreatedBy,
                        c.UpdatedBy,
                        c.Legal.LegalInfo,
                        Legal = c.Legal == null
            ? null
            : new { Id = c.Legal.Id, LegalInfo = c.Legal.LegalInfo },


                        Addresses = c.Addresses.Select(a => new
                        {
                            a.Id,
                            a.CountryId,
                            a.StateId,
                            a.CityId,
                            a.AddressLine
                        }),

                        Contacts = c.Contacts.Select(ct => new ContactModel
                        {
                            ContactType = ct.ContactType.Name,
                            ContactTypeId = ct.ContactType.Id,
                            ContactName = ct.Name,
                            Id = ct.Id,
                        }), 
                        Employees = c.CompanyEmployees.Select(ct => new
                        {
                            ct.Id,
                            ct.Department,
                            ct.FirstName,
                            ct.LastName,
                            Contacts = c.Contacts.Select(ct => new ContactModel
                            {
                                ContactType = ct.ContactType.Name,
                                ContactTypeId = ct.ContactType.Id,
                                ContactName = ct.Name,
                                Id = ct.Id,
                            }),
                            ct.Notes,
                            ct.Position,
                        }),
                     
                    

                        Activities = c.CompanyActivities.Select(ct => new ActivityModel
                        {
                            IndustryName = ct.Industry.Name,
                            ActivityName = ct.Activity.Name,
                            activityId = ct.Activity.Id,
                        }),
                    })
                    .FirstOrDefaultAsync();

                if (result == null)
                    return null;

                var files = await CTX.TbFiles
                    .Where(f => f.EntityId == id && f.EntityType == tableName.Company && !f.IsDeleted)
                    .Select(f => new
                    {
                        f.Id,
                        f.FileName,
                      
                    })
                    .ToListAsync();

                return new
                {
                    Company = result,
                    Files = files
                };
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<bool> Add(Co_Company_Name Company)
        {
            try
            {
                Company.CreatedAt = DateTime.Now;
                Company.IsDeleted = false;
                await CTX.Co_Companies.AddAsync(Company);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }

        //public async Task<bool> Delete(int id)
        //{
        //    try
        //    {
        //        // استخدم SQL مباشرة
        //        var result = await CTX.Database.ExecuteSqlRawAsync(
        //            "UPDATE Co_Companies SET IsDeleted = 1 WHERE Id = {0}", id);
        //        return result > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        await ClsLogs.Add("Error", ex.Message, null);
        //        return false;
        //    }
        //}
        public async Task<bool> Delete(int id)
        {
            try
            {
                var Company = await CTX.Co_Companies.FirstOrDefaultAsync(c => c.Id == id);
                if (Company == null)
                    return false;
                Company.IsDeleted = true;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> Edit(Co_Company_Name Company)
        {
            try
            {
                Company.UpdatedAt = DateTime.Now;
                CTX.Entry(Company).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<Co_Company_Name>> Search(string name, int page, int count)
        {
            try
            {
                var query = CTX.Co_Companies
                    .AsNoTracking()
                            .Include(c => c.Addresses)
                                .Include(c => c.Contacts)
                    .Where(a =>
                        !a.IsDeleted &&
                        (
                            string.IsNullOrWhiteSpace(name) ||
                            EF.Functions.Like(a.Name, $"%{name}%") ||
                            EF.Functions.Like(a.Id.ToString(), $"%{name}%") ||
                            EF.Functions.Like(a.CompanyDescription, $"%{name}%") ||
                            EF.Functions.Like(a.Comm_No, $"%{name}%") ||
                            EF.Functions.Like(a.Found_Date.ToString(), $"%{name}%") ||
                            EF.Functions.Like(a.Tax_No, $"%{name}%") ||
                          //  a.CompanyLegals.Any(l => EF.Functions.Like(l.Legal.LegalInfo, $"%{name}%")) ||
                            a.Contacts.Any(l => EF.Functions.Like(l.Name, $"%{name}%")) ||
                            a.Id.ToString().Contains(name)
                        )
                    );

                var pagedItems = await query
                    .OrderByDescending(a => a.Id)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToListAsync();

                return pagedItems;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int Count)
        {
            try
            {
                var LstCompany = await ClsHistory.GetAllHistory(Pageid, id, tableName.Company, Count);
                if (LstCompany == null)
                {
                    return new List<TbHistory>();
                }
                else
                {
                    return LstCompany;
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<List<object>> GetAllCustomersIdAndName()
        {
            try
            {
                var result = await CTX.Co_Companies
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => new {
                        x.Id,
                        x.Name
                    })
                    .ToListAsync();
                if (result == null)
                {
                    return null;
                }

                return result.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
      

        public async Task<int> Count( string? legalFilter)
        {
            try
            {

                var customer = await CTX.Co_Companies.AsNoTracking().Where(c => c.IsDeleted == false /*&&  c.CompanyLegals.Any(l => l.Legal.LegalInfo == legalFilter)*/).CountAsync();
                if (customer == null)
                    return 0;
                return customer;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return 0;
            }
        }
    }
}