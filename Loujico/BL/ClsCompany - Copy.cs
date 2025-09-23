using FuzzySharp;
using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;
using System.Linq;
namespace Loujico.BL

{
    public interface ICompanys
    {
        public  Task<List<Co_Company_Name>> GetAll(int id, int count, string? legalFilter);
        public Task<List<object>> GetAllCustomersIdAndName();
        public Task<CustomerModel> GetById(int id);
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
        const int pageSize = 10;
        public ClsCompany(CompanySystemContext companySystemContext, Ilog clsLogs, IHistory clsHistory)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
            ClsHistory = clsHistory;
        }

        public async Task<List<Co_Company_Name>> GetAll(int id, int count, string? legalFilter )
        {
            try
            {
                var query = CTX.Co_Companies
                               .AsNoTracking()
                               .Include(c => c.Addresses)
                               .Include(c => c.Contacts)
                               .Where(x => !x.IsDeleted);

                // إذا في فلترة على الـ legals
                if (!string.IsNullOrEmpty(legalFilter))
                {
                    query = query.Where(c => c.Legals.Any(l => l.LegalInfo == legalFilter));

                    // أو إذا بدك بحث جزئي:
                    // query = query.Where(x => x.Legals.Contains(legalFilter));
                }

                return await query.Skip((id - 1) * count)
                                  .Take(count)
                                  .ToListAsync();
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<CustomerModel> GetById(int id)
        {
            try
            {
                var cus = await CTX.Co_Companies
                                .AsNoTracking()
                                .Include(c => c.Legals)
                                .Include(c => c.Addresses)
                                .Include(c => c.Activity)
                                .Include(c => c.Contacts)
                               // .Include(c => c.TbInvoices)
                                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
                if (cus == null)
                {
                    return null;
                }
                var files = await CTX.TbFiles
                      .Where(f => f.EntityId == cus.Id && f.EntityType == tableName.Company && !f.IsDeleted)
                      .ToListAsync();
                var result = new CustomerModel
                {
                    Customer = cus,
                    Files = files,

                }; return result;
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
                            a.Legals.Any(l => EF.Functions.Like(l.LegalInfo, $"%{name}%")) ||
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
                var customer = await CTX.Co_Companies.AsNoTracking().Where(c => c.IsDeleted == false||  c.Legals.Any(l => l.LegalInfo == legalFilter)).CountAsync();
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