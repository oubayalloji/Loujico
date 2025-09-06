using Loujico.Models;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface ICustomers
    {
        public Task<List<TbCustomer>> GetAll(int id);
        public Task<TbCustomer> GetById(int id);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id);
        public Task<bool> Edit(TbCustomer customer);
        public Task<bool> Add(TbCustomer customer);
        public Task<bool> Delete(int id);
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

        public async Task<List<TbCustomer>> GetAll(int id)
        {
            try
            {
                return await CTX.TbCustomers
                                .Where(x => !x.IsDeleted).Skip((id - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            }
            catch
            {
                return new List<TbCustomer>();
            }
        }
        public async Task<TbCustomer?> GetById(int id)
        {
            return await CTX.TbCustomers
                            .Include(c => c.TbCustomersProducts)
                            .Include(c => c.TbProjects)
                            .Include(c => c.TbInvoices)
                            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
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
            catch
            {
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
            catch
            {
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
        public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id)
        {
            try
            {
                var LstCustomer = await ClsHistory.GetAllHistory(Pageid, id, "TbCustomer");
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
                return new List<TbHistory>();
            }
        }
    }
}