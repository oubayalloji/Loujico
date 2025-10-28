using FuzzySharp;
using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using System;
namespace Loujico.BL
{

    public interface IProject
    {
        public  Task<ShowProjectModel> GetById(int id);
        public  Task<ShowProject> GetByIdModel(int id);
        public  Task<List<object>> Pagintion(int id, int count);
        public Task<bool> Add(TbProject project);
        public Task<bool> Edit(TbProject project);
        public Task<bool> Delete(int id);
        public Task<int> Count();
        public Task<int> CountPending();

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

        public async Task<ShowProjectModel> GetById(int id)
        {
            try
            {
                var projectDto = await CTX.TbProjects
    .Where(p => p.Id == id && !p.IsDeleted)
    .Select(p => new ShowProjectModel
    {
        Id = p.Id,
        Title = p.Title,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        Progress = p.Progress,
        Price = p.Price,
        CompanyId = p.CompanyId,
        CompanyName=p.Company.Name,
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
                        p.Company.Name,
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
                    var Project = await CTX.TbProjects.FirstOrDefaultAsync(e => e.Id == id);
                    if (Project == null)
                        return false;

                Project.IsDeleted = true;
                    CTX.Entry(Project).State = EntityState.Modified; // استخدم هي للتعديل 
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
                var LstProject = await ClsHistory.GetAllHistory(Pageid, id, tableName.project, count);
                if (LstProject == null)
                {
                    return null;
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
                var query = CTX.TbProjects
                    .AsNoTracking()
                    .Where(a =>
                        !a.IsDeleted &&
                        (
                            string.IsNullOrWhiteSpace(name) ||
                            a.Id.ToString().Contains(name) ||
                            EF.Functions.Like(a.Title, $"%{name}%") ||
                            EF.Functions.Like(a.ProjectStatus, $"%{name}%") ||
                            a.Progress.ToString().Contains(name) ||
                            a.ProjectType.ToString().Contains(name) ||
                            a.Price.ToString().Contains(name) ||
                            a.StartDate.ToString().Contains(name) ||
                            a.EndDate.ToString().Contains(name)||
                               EF.Functions.Like(a.Company.Name, $"%{name}%")
                        )
                    );

                var pagedItems = await query
                    .OrderByDescending(a => a.Id)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToListAsync();

                return pagedItems;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }

        public async Task<int> CountPending()
        {
            try
            {
                var LstProject =await CTX.TbProjects.AsNoTracking().Where(a=>!a.IsDeleted&& a.ProjectStatus== "Pending").CountAsync();
                if (LstProject == null)
                {
                    return 0;
                }
                else
                {
                    return LstProject;
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return 0;
            }
        }
        public async Task<int> Count()
        {
            try
            {
                var LstProject =await CTX.TbProjects.AsNoTracking().Where(a=>!a.IsDeleted).CountAsync();
                if (LstProject == null)
                {
                    return 0;
                }
                else
                {
                    return LstProject;
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return 0;
            }
        }

        public async Task<ShowProject> GetByIdModel(int id)
        {
            try
            {
                var projectDto = await CTX.TbProjects
            .Where(p => p.Id == id && !p.IsDeleted)
    
            .Include(i => i.TbProjectsEmployees).Select(p => new
            {
                p.Id,
                p.Title,
                p.StartDate,
                p.EndDate,
                p.Progress,
                p.Price,
                p.CompanyId,
                Companyname= p.Company.Name,
                Employees = p.TbProjectsEmployees.Select(pe => new
                {
                    pe.EmployeeId,
                    pe.RoleOnProject,
                    pe.Employee.FirstName,
                    pe.Employee.LastName
                })
            }).FirstOrDefaultAsync();


                if (projectDto == null)
                    return null;

                // 2) جيب الملفات الخاصة بالموظف
                var files = await CTX.TbFiles
                    .Where(f => f.EntityId == projectDto.Id && f.EntityType == tableName.project && !f.IsDeleted)
                    .ToListAsync();

                // 3) جهّز الـ ViewModel
                var result = new ShowProject
                {
                    project = projectDto,
                    Files = files,

                }; // نوع الدالة Task<ProjectWithEmployeesDto>
                return result;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
    }
    }
