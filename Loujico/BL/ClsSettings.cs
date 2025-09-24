using Loujico.Models;
using Microsoft.EntityFrameworkCore;

namespace Loujico.BL
{
    public interface Isettings
    {
        public  Task<List<object>> GetAllContactType();
        public  Task<bool> DeleteContact(int Id);
        public Task<bool> AddContact(TbContact contact); 
        public  Task<List<object>> GetAllIndustryType();
        public  Task<bool> DeleteIndustry(int Id);
        public Task<bool> AddIndustry(Co_Industry Industry);
        public  Task<List<object>> GetAllLegalType();
        public  Task<bool> DeleteLegal(int Id);
        public Task<bool> AddLegal(Co_Legal Legal);
    }
    public class ClsSettings : Isettings
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;
        IHistory ClsHistory;

        public ClsSettings(CompanySystemContext companySystemContext,Ilog clsLogs,IHistory clsHistory)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
            ClsHistory = clsHistory;
        }

        #region Industry
        public async Task<bool> AddIndustry(Co_Industry Industry)
        {
            try
            {
                await CTX.Co_Industries.AddAsync(Industry);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> DeleteIndustry(int Id)
        {
            try
            {
               var Industry= await CTX.Co_Industries.FirstOrDefaultAsync(x => x.Id== Id);
                if (Industry==null)
                {
                    return false;
                }
                CTX.Co_Industries.Remove(Industry);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<object>> GetAllIndustryType()
        {
            try
            {
                var result = await CTX.Co_Industries
                    .AsNoTracking()
                    .Select(x => new
                    {
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

        #endregion

        #region Legal
        public async Task<bool> AddLegal(Co_Legal Legal)
        {
            try
            {
                await CTX.Co_Legals.AddAsync(Legal);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> DeleteLegal(int Id)
        {
            try
            {
                var Legal = await CTX.Co_Legals.FirstOrDefaultAsync(x => x.Id == Id);
                if (Legal == null)
                {
                    return false;
                }
                CTX.Co_Legals.Remove(Legal);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<object>> GetAllLegalType()
        {
            try
            {
                var result = await CTX.Co_Legals
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.LegalInfo
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
        #endregion

        #region contact

        public async Task<bool> AddContact(TbContact contact)
        {
            try
            {
                await CTX.TbContact.AddAsync(contact);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> DeleteContact(int Id)
        {
            try
            {
                var contact = await CTX.TbContact.FirstOrDefaultAsync(x => x.Id == Id);
                if (contact == null)
                {
                    return false;
                }
                CTX.TbContact.Remove(contact);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<object>> GetAllContactType()
        {
            try
            {
                var result = await CTX.TbContact
                    .AsNoTracking()
                    .Select(x => new
                    {
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
        #endregion
    }
}
