using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using System;
namespace Loujico.BL
{

    public interface IProject
    {
        public Task<TbProject> GetById(int id);
        public Task<List<TbProject>> Pagintion(int id);
        public Task<bool> Add(TbProject project);
        public Task<bool> Delete(int id);


    }

    public class ClsProject : IProject
    {
        CompanySystemContext CTX;
        ClsLogs ClsLogs { get; set; }
        const int pageSize = 10;

        public ClsProject(CompanySystemContext companySystemContext, ClsLogs clsLogs)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
        }
        public async Task<List<TbProject>> GetAll(int id)
        {
            try
            {
                var lstProj = await CTX.TbProjects
                                            .Where(e => !e.IsDeleted).Skip((id - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();
                return lstProj;
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return new List<TbProject>();


            }
        }

        public async Task<TbProject> GetById(int id)
        {
            var project = await CTX.TbProjects.FindAsync(id);
            if (project == null)
            {
                return new TbProject();
            }
            return project;
        }

        public async Task<List<TbProject>> Pagintion(int id)
        {
            try
            {
                var LstCars = await CTX.TbProjects
                                       .Where(a => a.IsDeleted != true)
                                       .Skip((id - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

                return LstCars;
            }
            catch
            {
                return new List<TbProject>();
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
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {

            try
            {
                var project = await CTX.TbProjects.FindAsync(id);
                if (project == null)
                    return false;

                project.UpdatedAt = DateTime.Now;
                project.IsDeleted = true;
                CTX.Entry(project).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}