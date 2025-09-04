using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using System;
namespace Loujico.BL
{
    public interface IEmployees
    {
        public Task<List<TbEmployee>> GetAllEmployees(int id);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id);
        public Task<bool> Edit(TbEmployee employee);
        public Task<TbEmployee> GetEmployeeById(int id);
        public Task<bool> Add(TbEmployee employee);
        public Task<bool> Delete(int id);
    }

    public class ClsEmployees : IEmployees
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;
        IHistory ClsHistory;
        const int pageSize = 10;
        public ClsEmployees(CompanySystemContext companySystemContext, Ilog clsLogs, IHistory history)
        {
            ClsHistory = history;
            CTX = companySystemContext;
            ClsLogs = clsLogs;
        }

        public async Task<List<TbEmployee>> GetAllEmployees(int id)
        {
            try
            {
                var lstEmployees = await CTX.TbEmployees
                                            .Where(e => !e.IsDeleted).Skip((id - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();
                return lstEmployees;
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return new List<TbEmployee>();


            }
        }

        public async Task<bool> Add(TbEmployee employee)
        {
            try
            {
                employee.CreatedAt = DateTime.Now;
                employee.IsPresent = true;
                await CTX.TbEmployees.AddAsync(employee);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<bool> Edit(TbEmployee employee)
        {
            try
            {
                employee.UpdatedAt = DateTime.Now;
                
                CTX.Entry(employee).State = EntityState.Modified;
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
                var employee = await CTX.TbEmployees.FirstOrDefaultAsync(e => e.Id == id);
                if (employee == null)
                    return false;

                employee.IsDeleted = true;
                CTX.Entry(employee).State = EntityState.Modified; // استخدم هي للتعديل 
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }

        public async Task<TbEmployee> GetEmployeeById(int id)
        {
            try
            {
                return await CTX.TbEmployees.Where(a => a.IsDeleted == false)
                                .Include(e => e.TbProjectsEmployees)
                                .Include(e => e.TbProductsEmployees)
                                .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return new TbEmployee();
            }
        }

        public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id)
        {
            try
            {

                var LstEmployee = await ClsHistory.GetAllHistory(Pageid, id, "TbEmployees");
                if (LstEmployee != null)
                {
                    return new List<TbHistory>();

                }
                else
                {
                    return LstEmployee;
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return new List<TbHistory>();
            }
        }
    }
}