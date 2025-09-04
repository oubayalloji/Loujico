using Loujico.Models;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface ICustomers
    {
        public Task<List<TbCustomer>> GetAllCustomersAsync(int id);
        public Task<TbCustomer?> GetCustomerByIdAsync(int id);
        public Task<bool> AddCustomerAsync(TbCustomer customer);
        public Task<bool> DeleteAsync(int id);
    }
    public class ClsCustomers : ICustomers
    {
        CompanySystemContext CTX;
        const int pageSize = 10;
        public ClsCustomers(CompanySystemContext companySystemContext)
        {
            CTX = companySystemContext;
        }

        public async Task<List<TbCustomer>> GetAllCustomersAsync(int id)
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
        public async Task<TbCustomer?> GetCustomerByIdAsync(int id)
        {
            return await CTX.TbCustomers
                            .Include(c => c.TbCustomersProducts)
                            .Include(c => c.TbProjects)
                            .Include(c => c.TbInvoices)
                            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
        public async Task<bool> AddCustomerAsync(TbCustomer customer)
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
        public async Task<bool> DeleteAsync(int id)
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
    }
}