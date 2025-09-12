using FuzzySharp;
using Loujico.Models;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface IInvoices
    {
        public Task<List<TbInvoice>> GetAll(int id, int count);
        public Task<InvoiceModel> GetById(int id);
        public Task<bool> Add(TbInvoice invoice);
        public Task<bool> Delete(int id);
        public Task<bool> Edit(TbInvoice invoice);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count);
        public Task<List<TbInvoice>> Search(string name, int page, int count);
        public Task<int> Count();
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
       
        public async Task<List<TbInvoice>> GetAll(int id, int count)
        {
            try
            {
                return await CTX.TbInvoices
                                .Where(i => !i.IsDeleted)
                                .Skip((id - 1) * count)
                                .Take(count)
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
        public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count)
        {
            try
            {
                var LstInvoice = await ClsHistory.GetAllHistory(Pageid, id, "TbInvoices", count);
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
        public async Task<List<TbInvoice>> Search(string name, int page, int count)
        {
            try
            {
                // جلب البيانات أولاً من قاعدة البيانات بدون تتبع 
                var allItems = await CTX.TbInvoices
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted)
                    .ToListAsync();

                // تطبيق المطابقة التقريبية باستخدام FuzzySharp
                var matchedItems = allItems.Where(a =>
                    Fuzz.PartialRatio(name, a.InvoiceStatus) > 70 ||
                    Fuzz.PartialRatio(name, a.Customer.ToString()) > 70 ||
                    Fuzz.Ratio(name, a.Amount.ToString()) > 70 ||
                    Fuzz.Ratio(name, a.DueDate.ToString()) > 70 ||
                    Fuzz.Ratio(name, a.InvoicesDate.ToString()) > 70 ||
                    Fuzz.Ratio(name, a.ProjectId.ToString()) > 70 ||
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
        public async Task<int> Count()
        {
            try
            {
                var Invoice = await CTX.TbInvoices.AsNoTracking().Where(c => c.IsDeleted == false && c.InvoiceStatus== "Overdue").CountAsync();
                if (Invoice == null)
                    return 0;
                return Invoice;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return 0;
            }
        }
    }
}