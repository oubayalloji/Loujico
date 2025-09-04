using Loujico.Models;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface IInvoices
    {
        public Task<List<TbInvoice>> GetAllInvoicesAsync(int id);
        public Task<TbInvoice> GetInvoiceByIdAsync(int id);
        public Task<bool> AddInvoiceAsync(TbInvoice invoice);
        public Task<bool> DeleteAsync(int id);
    }
    public class ClsInvoices : IInvoices
    {
        CompanySystemContext CTX;
        const int pageSize = 10;
        public ClsInvoices(CompanySystemContext companySystemContext)
        {
            CTX = companySystemContext;
        }
        public async Task<List<TbInvoice>> GetAllInvoicesAsync(int id)
        {
            try
            {
                return await CTX.TbInvoices
                                .Where(i => !i.IsDeleted)
                                .Skip((id - 1) * pageSize)
                                .Take(pageSize)
                                .Include(i => i.Customer)
                                .Include(i => i.Project)
                                .ToListAsync();
            }
            catch
            {
                return new List<TbInvoice>();
            }
        }
        public async Task<TbInvoice> GetInvoiceByIdAsync(int id)
        {
            return await CTX.TbInvoices
                            .Include(i => i.Customer)
                            .Include(i => i.Project)
                            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }
        public async Task<bool> AddInvoiceAsync(TbInvoice invoice)
        {
            try
            {
                invoice.CreatedAt = DateTime.Now;
                invoice.IsDeleted = false;
                await CTX.TbInvoices.AddAsync(invoice);
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
                var invoice = await CTX.TbInvoices.FirstOrDefaultAsync(i => i.Id == id);
                if (invoice == null)
                    return false;

                invoice.IsDeleted = true;
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