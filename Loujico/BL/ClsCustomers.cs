using FuzzySharp;
using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;
namespace Loujico.BL

{
    public interface ICustomers
    {
        public Task<List<TbCustomer>> GetAll(int id,int count);
        public  Task<List<object>> GetAllCustomersIdAndName();
        public Task<CustomerModel> GetById(int id);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count);
        public Task<bool> Edit(TbCustomer customer);
        public Task<bool> Add(TbCustomer customer);
        public Task<bool> Delete(int id);
        public Task<List<TbCustomer>> Search(string name, int page, int count);
    }
    public class ClsCustomers : ICustomers
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;
        IHistory ClsHistory;
        const int pageSize = 10;
        public ClsCustomers(CompanySystemContext companySystemContext, Ilog clsLogs, IHistory clsHistory)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
            ClsHistory = clsHistory;
        }

        public async Task<List<TbCustomer>> GetAll(int id,int count)
        {
            try
            {
                return await CTX.TbCustomers.AsNoTracking()
                                .Where(x => !x.IsDeleted).Skip((id - 1) * count)
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
                var cus = await CTX.TbCustomers
                                .Include(c => c.TbCustomersProducts)
                                .Include(c => c.TbProjects)
                                .Include(c => c.TbInvoices)
                                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
                if (cus == null)
                {
                    return null;
                }
                var files = await CTX.TbFiles
                      .Where(f => f.EntityId == cus.Id && f.EntityType == tableName.Employee && !f.IsDeleted)
                      .ToListAsync();
                var result = new CustomerModel
                {
                    Customer = cus,
                    Files = files,

                };

                return result;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<bool> Add(TbCustomer customer)
        {
            try
            {
                customer.CreatedAt = DateTime.Now;
                customer.IsDeleted = false;
                await CTX.TbCustomers.AddAsync(customer);
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
                var customer = await CTX.TbCustomers.FirstOrDefaultAsync(c => c.Id == id);
                if (customer == null)
                    return false;
                customer.IsDeleted = true;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> Edit(TbCustomer customer)
        {
            try
            {
                customer.UpdatedAt = DateTime.Now;
                CTX.Entry(customer).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
   

public async Task<List<TbCustomer>> Search(string name, int page, int count)
    {
        try
        {
            // جلب البيانات أولاً من قاعدة البيانات بدون تتبع
            var allItems = await CTX.TbCustomers
                .AsNoTracking()
                .Where(a => !a.IsDeleted)
                .ToListAsync();

            // تطبيق المطابقة التقريبية باستخدام FuzzySharp
            var matchedItems = allItems.Where(a =>
                Fuzz.PartialRatio(name, a.CustomerName) > 70 ||
                Fuzz.PartialRatio(name, a.CustomerAddress) > 70 ||
                Fuzz.PartialRatio(name, a.Industry ?? "") > 70 ||
                Fuzz.Ratio(name, a.Phone) > 70 ||
                Fuzz.PartialRatio(name, a.ServiceProvided ?? "") > 70 ||
                Fuzz.Ratio(name, a.Id.ToString()) > 70
            );

            // تطبيق الـ pagination
            var pagedItems = matchedItems
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            return pagedItems;
        }
        catch (Exception ex)
        {
            await ClsLogs.Add("Error", ex.Message, null);
            return null;
        }
    }
    public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id,int Count)
        {
            try
            {
                var LstCustomer = await ClsHistory.GetAllHistory(Pageid, id, tableName.Customer,Count);
                if (LstCustomer == null)
                {
                    return new List<TbHistory>();

                }
                else
                {
                    return LstCustomer;
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
                var result = await CTX.TbCustomers
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => new {
                        x.Id,
                        x.CustomerName
                    })
                    .ToListAsync();
                if (result==null)
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
    }
}