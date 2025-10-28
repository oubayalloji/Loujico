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
        public  Task<List<object>> GetAllActivityType();
        public  Task<bool> DeleteActivity(int Id);
        public Task<bool> AddActivity(Co_Activity Legal);
        public Task<List<object>> GetActivityByIndustry(int Id);
        public Task<bool> AddState(TbState State);
        public  Task<bool> DeleteState(int Id);
        public  Task<List<object>> GetAllStateType();
        public  Task<List<object>> GetStateByCountry(int Id);
        public Task<bool> AddCity(TbCity City);
        public Task<bool> DeleteCity(int Id);
        public Task<List<object>> GetAllCityType();
<<<<<<< HEAD
        public Task<List<object>> GetCityByState(int Id);
=======
        public Task<List<object>> GetCityByIndustry(int Id);
>>>>>>> origin/boss
        public Task<bool> AddCountry(TbCountry Country);
        public Task<bool> DeleteCountry(int Id);
        public Task<List<object>> GetAllCountryType();

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
        #region State

        public async Task<bool> AddState(TbState State)
        {
            try
            {
                await CTX.TbStates.AddAsync(State);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> DeleteState(int Id)
        {
            try
            {
                var State = await CTX.TbStates.FirstOrDefaultAsync(x => x.Id == Id);
                if (State == null)
                {
                    return false;
                }
                CTX.TbStates.Remove(State);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<object>> GetAllStateType()
        {
            try
            {
                var result = await CTX.TbStates
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
 

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
        public async Task<List<object>> GetStateByCountry(int Id)
        {
            try
            {
                var result = await CTX.TbStates
                    .AsNoTracking()
                    .Where(a => a.CountrId == Id)
                    .Select(a => new
                    {
                        a.Id,
                        a.Name
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


        #region City

        public async Task<bool> AddCity(TbCity City)
        {
            try
            {
                await CTX.TbCities.AddAsync(City);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> DeleteCity(int Id)
        {
            try
            {
                var City = await CTX.TbCities.FirstOrDefaultAsync(x => x.Id == Id);
                if (City == null)
                {
                    return false;
                }
                CTX.TbCities.Remove(City);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<object>> GetAllCityType()
        {
            try
            {
                var result = await CTX.TbCities
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,


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
        public async Task<List<object>> GetCityByState(int Id)
        {
            try
            {
                var result = await CTX.TbCities
                    .AsNoTracking()
                    .Where(a => a.StateId == Id)
                    .Select(a => new
                    {
                        a.Id,
                        a.Name
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

        #region Activity

        public async Task<bool> AddActivity(Co_Activity Activity)
        {
            try
            {
                await CTX.Co_Activities.AddAsync(Activity);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> DeleteActivity(int Id)
        {
            try
            {
                var contact = await CTX.Co_Activities.FirstOrDefaultAsync(x => x.Id == Id);
                if (contact == null)
                {
                    return false;
                }
                CTX.Co_Activities.Remove(contact);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<object>> GetAllActivityType()
        {
            try
            {
                var result = await CTX.Co_Activities
                    .AsNoTracking()
                    .Include(x => x.CompanyActivities)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        Industry = x.Industry.Name
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
        public async Task<List<object>> GetActivityByIndustry(int Id)
        {
            try
            {
                var result = await CTX.Co_Activities
                    .AsNoTracking()
                    .Where(a => a.IndustryId == Id)
                    .Select(a => new
                    {
                        a.Id,
                        a.Name
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

        #region Country
        public async Task<bool> AddCountry(TbCountry Country)
        {
            try
            {
                await CTX.TbCountries.AddAsync(Country);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<object>> GetAllCountryType()
        {
            try
            {
                var result = await CTX.TbCountries
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
        public async Task<bool> DeleteCountry(int Id)
        {
            try
            {
                var Country = await CTX.TbCountries.FirstOrDefaultAsync(x => x.Id == Id);
                if (Country == null)
                {
                    return false;
                }
                CTX.TbCountries.Remove(Country);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }

        #endregion
    }
}
