using Microsoft.EntityFrameworkCore.ChangeTracking;
using Loujico.Models;
using Loujico.BL;
using Microsoft.EntityFrameworkCore;

namespace Loujico.BL
{
    public interface Ilog
    {
        public Task<string> Add(string ActionType, string Action, String? Userid);
        public Task<List<TbLog>> Paginition(int id, int count);
        public Task<int> Count();

        public  Task<List<TbLog>> Search(string name, int page, int count);


    }
    public class ClsLogs : Ilog
    {
        CompanySystemContext CTX;
        const int pageSize = 10;
       
        public ClsLogs(CompanySystemContext companySystemContext)
        {
            CTX = companySystemContext;
        }
        public async Task<string> Add(string ActionType, string Action, String? Userid)
        {
            try
            {
                TbLog log = new TbLog
                {
                    UserId = Userid,
                    ActionType = ActionType,
                    Action = Action,
                    TimeStamp = DateTime.Now,

                };
                CTX.AddAsync(log);
                CTX.SaveChanges();
                return "Done";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<List<TbLog>> Paginition(int id, int count)
        {
            try
            {
                var LstCars = await CTX.TbLogs.Where(a => a.TimeStamp >= DateTime.Now.AddMonths(-3)).Skip((id - 1) * count)
                .Take(count)
                .ToListAsync();
                return LstCars;
            }
            catch (Exception ex)
            {
                TbLog log = new TbLog
                {

                    ActionType = "Error",
                    Action = ex.Message,
                    TimeStamp = DateTime.Now,

                };
                CTX.AddAsync(log);
                CTX.SaveChangesAsync();
                return new List<TbLog>();
            }


        }
        public async Task<int> Count()
        {
            try
            {
                var log = await CTX.TbLogs.AsNoTracking().CountAsync();
                if (log == null)
                    return 0;
                return log;
            }
            catch (Exception ex)
            {
                await Add("Error", ex.Message, null);
                return 0;
            }
        }
        public async Task<List<TbLog>> Search(string name, int page, int count)
        {
            try
            {
                var query = CTX.TbLogs
                    .AsNoTracking()
                    .Where(a =>
                        (
                            string.IsNullOrWhiteSpace(name) ||
                            EF.Functions.Like(a.ActionType, $"%{name}%")
                            
                       
                        )
                    );

                var pagedItems = await query
                    .OrderByDescending(a => a.Id)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToListAsync();

                return pagedItems.Any() ? pagedItems : null;
            }
            catch (Exception ex)
            {
                await Add("Error", ex.Message, null);
                return null;
            }
        }
    }
}