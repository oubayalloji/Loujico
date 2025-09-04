using Microsoft.EntityFrameworkCore.ChangeTracking;
using Loujico.Models;
using Loujico.BL;
using Microsoft.EntityFrameworkCore;

namespace Loujico.BL
{
    public interface Ilog
    {
        public Task<string> Add(string ActionType, string Action, String? Userid);
        public Task<List<TbLog>> Paginition(int id);




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
        public async Task<List<TbLog>> Paginition(int id)
        {
            try
            {
                var LstCars = await CTX.TbLogs.Where(a => a.TimeStamp >= DateTime.Now.AddMonths(-3)).Skip((id - 1) * pageSize)
                .Take(pageSize)
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
    }
}