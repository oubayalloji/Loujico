using FuzzySharp;
using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using System;
namespace Loujico.BL
{
    public interface IEmployees
    {
        public Task<List<TbEmployee>> GetAllEmployees(int id, int count);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count);
        public Task<bool> Edit(TbEmployee employee);
        public Task<ShowEmployeeModel> GetById(int id);
        public Task<bool> Add(TbEmployee employee);
        public Task<bool> Delete(int id);
        public Task<string> DeActive(int id);
        public Task<string> Active(int id);
        public Task<List<TbEmployee>> Search(string name, int page, int count);
        public Task<List<object>> GetAllEmployeesIdAndName();



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

        public async Task<List<TbEmployee>> GetAllEmployees(int id, int count)
        {
            try
            {
                var lstEmployees = await CTX.TbEmployees.AsNoTracking()
                                            .Where(e => !e.IsDeleted).Skip((id - 1) * count)
                                            .Take(count)
                                            .ToListAsync();
                return lstEmployees;
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return new List<TbEmployee>();


            }
        }
        public async Task<List<object>> GetAllEmployeesIdAndName()
        {
            try
            {
                var result = await CTX.TbEmployees                 
                    .Where(x => !x.IsDeleted)
                    .Select(x => new {
                        x.Id,
                        x.FirstName,
                        x.LastName
                    })
                    .ToListAsync();
                if (result == null)
                    return null;
                    return result.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
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

        public async Task<ShowEmployeeModel> GetById(int id)
        {
            try
            {
                // 1) جيب الموظف مع علاقاته
                var employee = await CTX.TbEmployees
                    .Include(e => e.TbProjectsEmployees)
                    .Include(e => e.TbProductsEmployees)
                    .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

                if (employee == null)
                    return null;

                // 2) جيب الملفات الخاصة بالموظف
                var files = await CTX.TbFiles
                    .Where(f => f.EntityId == employee.Id && f.EntityType == tableName.Employee && !f.IsDeleted)
                    .ToListAsync();

                // 3) جهّز الـ ViewModel
                var result = new ShowEmployeeModel
                {
                    Employee = employee,
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


        public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id,int count)
        {
            try
            {

                var LstEmployee = await ClsHistory.GetAllHistory(Pageid, id, "TbEmployees", count);
                if (LstEmployee == null)
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
        public async Task<string> DeActive(int id)
        {
            try
            {
                var employee = await CTX.TbEmployees.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (employee == null || employee.IsDeleted || !employee.IsPresent)
                    return "الموظف غير موجود أو معطل مسبقًا";


                employee.IsPresent = false;
                CTX.Entry(employee).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return "تم التعطيل";
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return "خطأ أثناء التعطيل";
            }
        }
        public async Task<string> Active(int id)
        {
            try
            {
                var employee = await CTX.TbEmployees.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
                if (employee == null || employee.IsDeleted || employee.IsPresent)
                    return "الموظف غير موجود أو مفعّل مسبقًا";
                employee.IsPresent = true;
                CTX.Entry(employee).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return "تم التفعيل";
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return "خطأ أثناء التفعيل";
            }
        }
        public async Task<List<TbEmployee>> Search(string name, int page, int count)
        {
            try
            {
                // جلب البيانات أولاً من قاعدة البيانات بدون تتبع
                var allItems = await CTX.TbEmployees
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted)
                    .ToListAsync();

                // تطبيق المطابقة التقريبية باستخدام FuzzySharp
                var matchedItems = allItems.Where(a =>
                    Fuzz.PartialRatio(name, a.FirstName) > 70 ||
                    Fuzz.PartialRatio(name, a.LastName) > 70 ||
                    Fuzz.PartialRatio(name, a.Position) > 70 ||
                    Fuzz.Ratio(name, a.Salary.ToString()) > 70 ||
                    Fuzz.Ratio(name, a.Age.ToString()) > 70 ||
                    Fuzz.Ratio(name, a.Phone) > 70 ||
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
    }
}