using FuzzySharp;
using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using System;
namespace Loujico.BL
{

    public interface IProject
    {
        public  Task<AddProjectModel> GetById(int id);
        public  Task<List<object>> Pagintion(int id, int count);
        public Task<bool> Add(TbProject project);
        public Task<bool> Edit(TbProject project);
        public Task<bool> Delete(int id);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count);
        public Task<List<TbProject>> Search(string name, int page, int count);
        public Task<List<object>> GetAllProjectAndInvoice();

    }

    public class ClsProject : IProject
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;
        const int pageSize = 10;
        IHistory ClsHistory;

        public ClsProject(CompanySystemContext companySystemContext, Ilog clsLogs, IHistory clsHistory)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
            ClsHistory = clsHistory;
        }
        public async Task<bool> Edit(TbProject project)
        {
            try
            {
                project.UpdatedAt = DateTime.Now;

                CTX.Entry(project).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }

        public async Task<List<object>> GetAllProjectAndInvoice()
        {
            try
            {
                var result = await CTX.TbProjects
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => new {
                        x.Id,
                        x.Title
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

        public async Task<AddProjectModel> GetById(int id)
        {
            try
            {
                var projectDto = await CTX.TbProjects
    .Where(p => p.Id == id && !p.IsDeleted)
    .Select(p => new AddProjectModel
    {
        Id = p.Id,
        Title = p.Title,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        Progress = p.Progress,
        Price = p.Price,
        Employees = p.TbProjectsEmployees.Select(pe => new EmployeeOnProjectModel
        {
            EmployeeId = pe.EmployeeId,
            RoleOnProject = pe.RoleOnProject,
     
        }).ToList()
    }).FirstOrDefaultAsync();

                return projectDto; // نوع الدالة Task<ProjectWithEmployeesDto>

            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }


        public async Task<List<object>> Pagintion(int id, int count)
        {
            try
            {
                var projects = await CTX.TbProjects
                    .Where(p => !p.IsDeleted)
                    .Skip((id - 1) * count)
                    .Take(count)
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.StartDate,
                        p.EndDate,
                        p.Progress,
                        p.Price,
                        Employees = p.TbProjectsEmployees.Select(pe => new {
                            pe.EmployeeId,
                            pe.RoleOnProject,
                            pe.Employee.FirstName,
                            pe.Employee.LastName
                        })
                    }).ToListAsync();

                return projects.Cast<object>().ToList();
                /*   var LstCars = await CTX.TbProjects
                            .Where(a => !a.IsDeleted)
                            .Include(p => p.TbProjectsEmployees)        // جلب جدول الوسيط
                                .ThenInclude(pe => pe.Employee)        // جلب بيانات الموظف لكل علاقة
                            .Skip((id - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

                   return LstCars;*/

            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return new List<object>();
            }
        }

        public async Task<bool> Add(TbProject project)
        {
            try
            {
                project.CreatedAt = DateTime.Now;
                await CTX.TbProjects.AddAsync(project);
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
        public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count)
        {
            try
            {
                var LstProject = await ClsHistory.GetAllHistory(Pageid, id, "TbProject", count);
                if (LstProject != null)
                {
                    return new List<TbHistory>();
                }
                else
                {
                    return LstProject;
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return new List<TbHistory>();
            }
        }
        public async Task<List<TbProject>> Search(string name, int page, int count)
        {
            try
            {
                // جلب البيانات أولاً من قاعدة البيانات بدون تتبع
                var allItems = await CTX.TbProjects
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted)
                    .ToListAsync();

                // تطبيق المطابقة التقريبية باستخدام FuzzySharp
                var matchedItems = allItems.Where(a =>
                    Fuzz.Ratio(name, a.Id.ToString()) > 70 ||
                    Fuzz.PartialRatio(name, a.Title) > 70 ||
                    Fuzz.PartialRatio(name, a.ProjectStatus) > 70 ||
                    Fuzz.PartialRatio(name, a.Progress.ToString()) > 70 ||
                    Fuzz.PartialRatio(name, a.ProjectType.ToString()) > 70 ||
                    Fuzz.Ratio(name, a.Price.ToString()) > 70 ||
                    Fuzz.Ratio(name, a.StartDate.ToString()) > 50 || 
                    Fuzz.Ratio(name, a.EndDate.ToString()) > 50
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