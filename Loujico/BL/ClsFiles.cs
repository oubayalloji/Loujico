using Loujico.Models;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface IFiles
    {
        public Task<bool> Add(TbFile file);
        public Task<bool> Delete(int id);
    }

    public class ClsFiles : IFiles
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;

        public ClsFiles(CompanySystemContext companySystemContext, Ilog clsLogs)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
        }

        public async Task<bool> Add(TbFile file)
        {
            try
            {
                file.UploadedAt = DateTime.Now;
                file.IsDeleted = false;
                await CTX.TbFiles.AddAsync(file);
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
                var file = await CTX.TbFiles.FirstOrDefaultAsync(f => f.Id == id);
                if (file == null)
                    return false;
                file.IsDeleted = true;
                CTX.Entry(file).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
    }
}