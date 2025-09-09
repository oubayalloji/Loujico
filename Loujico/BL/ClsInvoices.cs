using Loujico.Models;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface IInvoices
    {
        public Task<List<TbInvoice>> GetAll(int id);
        public Task<InvoiceModel> GetById(int id);
        public Task<bool> Add(TbInvoice invoice);
        public Task<bool> Delete(int id);
        public Task<bool> Edit(TbInvoice invoice);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id);

    }
    public class ClsInvoices : IInvoices
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;
        IHistory ClsHistory;
        const int pageSize = 10;
        public ClsInvoices(CompanySystemContext companySystemContext)
        {
            CTX = companySystemContext;
        }
        public async Task<List<TbInvoice>> GetAll(int id)
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
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<InvoiceModel> GetById(int id)
        {
            try
            {

                var invoice = await CTX.TbInvoices
                                .Include(i => i.Customer)
                                .Include(i => i.Project)
                                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
                if (invoice == null)
                {
                    return null;
                }
                var files = await CTX.TbFiles
                      .Where(f => f.EntityId == invoice.Id && f.EntityType == tableName.invoice && !f.IsDeleted)
                      .ToListAsync();
                var result = new InvoiceModel
                {
                    Invoice = invoice,
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
        public async Task<bool> Add(TbInvoice invoice)
        {
            try
            {
                invoice.CreatedAt = DateTime.Now;
                invoice.IsDeleted = false;
                await CTX.TbInvoices.AddAsync(invoice);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return  false;
            }
        }
        public async Task<bool> Delete(int id)
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
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> Edit(TbInvoice invoice)
        {
            try
            {
                invoice.UpdatedAt = DateTime.Now;
                CTX.Entry(invoice).State = EntityState.Modified;
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
                var LstInvoice = await ClsHistory.GetAllHistory(Pageid, id, "TbInvoices");
                if (LstInvoice != null)
                {
                    return new List<TbHistory>();
                }
                else
                {
                    return LstInvoice;
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
    }
}