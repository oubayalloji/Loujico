using Loujico.Models;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface IHistory
    {
        public Task<List<TbHistory>> GetAllHistory(int id, int RecordId, string TableName);
        public Task<TbHistory> GetHistoryById(int id);
    }
    public class ClsHistory : IHistory
    {
        Ilog ClsLogs;
        CompanySystemContext CTX;
        const int pageSize = 10;
        public ClsHistory(CompanySystemContext companySystemContext, Ilog clsLogs)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
        }

        public async Task<List<TbHistory>> GetAllHistory(int id, int RecordId, string TableName)
        {
            try
            {
                return await CTX.TbHistories
                                .Where(a => a.RecordId == RecordId && a.TableName == TableName)
                                .Skip((id - 1) * pageSize)
                                .Take(pageSize)
                                .OrderByDescending(h => h.ActionTime)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<TbHistory> GetHistoryById(int id)
        {
            try
            {
                return await CTX.TbHistories
                                .FirstOrDefaultAsync(h => h.Id == id);
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
    }
}