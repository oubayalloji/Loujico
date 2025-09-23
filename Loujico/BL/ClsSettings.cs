using Loujico.Models;
using Microsoft.EntityFrameworkCore;

namespace Loujico.BL
{
    public interface Isettings
    {
        public  Task<List<object>> GetAllContactType();
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
        public async Task<bool> DeleteIndustry(Co_Industry Industry)
        {
            try
            {
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
        public async Task<bool> DeleteLegal(Co_Legal Legal)
        {
            try
            {
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
        public async Task<bool> DeleteContact(TbContact contact)
        {
            try
            {
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
