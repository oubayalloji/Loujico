using FuzzySharp;
using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface IProducts
    {
        public Task<List<object>> GetAllProducts(int id, int count);
        public Task<ProductModel?> GetById(int id);
        public Task<TbProduct?> GetByIdModel(int id);
        public Task<bool> Add(TbProduct product);
        public Task<bool> Edit(TbProduct product);
        public Task<bool> Delete(int id);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id, int count);
        public Task<List<TbProduct>> Search(string name, int page, int count);
        public Task<int> Count();
    }

    public class ClsProducts : IProducts
    {
        CompanySystemContext CTX;
        const int pageSize = 10;
        Ilog ClsLogs;
        IHistory ClsHistory;

        public ClsProducts(CompanySystemContext companySystemContext, Ilog clsLogs, IHistory clsHistory)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
            ClsHistory = clsHistory;
        }
        public async Task<TbProduct> GetByIdModel(int id)
        {
            try
            {
                var projectDto = await CTX.TbProducts.Where(p => p.Id == id  && !p.IsDeleted).FirstOrDefaultAsync();
                    if (projectDto == null)
                {
                    return null;
                }

                return projectDto; // نوع الدالة Task<ProjectWithEmployeesDto>

            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }

        public async Task<bool> Edit(TbProduct product)
        {
            try
            {
                product.UpdatedAt = DateTime.Now;

                CTX.Entry(product).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<List<object>> GetAllProducts(int id, int count)
        {
            try
            {
                var products = await CTX.TbProducts
                    .Where(p => !p.IsDeleted)
                    .Skip((id - 1) * count)
                    .Take(count)
                    .Select(p => new
                    {
                        p.Id,
                        p.ProductName,
                        p.BillingCycle,
                        p.IsActive,
                        p.Price
                    })
                    .ToListAsync();

                return products.Cast<object>().ToList(); // تحويل إلى List<object> إذا كان مطلوبًا
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return new List<object>(); // إرجاع قائمة فارغة بدلًا من null
            }
        }

        public async Task<ProductModel?> GetById(int id)
        {
            try
            {
                var Product = await CTX.TbProducts.Where(p => p.Id == id  && !p.IsDeleted)
                                .Include(p => p.TbCompanyProducts)
                                .Include(i => i.TbProductsEmployees).Select(p => new
                                {
                                    p.Id,
                                    p.BillingCycle,
                                    p.ProductDescription,
                                    p.ProductName,
                                    p.IsActive,
                                    p.Price,
                                    p.CreatedBy,
                                    p.CreatedAt,
                                    p.UpdatedBy,
                                    p.UpdatedAt,
                                    Employees = p.TbProductsEmployees.Select(pe => new
                                    {
                                        pe.EmployeeId,
                                       RoleOnProject= pe.RoleOnProduct,
                                        pe.Employee.FirstName,
                                        pe.Employee.LastName,
                                        pe.IsDeleted
                                    }).Where(a=>a.IsDeleted==false),
                                    Company = p.TbCompanyProducts.Select(co=> new
                                    {
                                        co.StartDate,
                                        co.EndDate,
                                        Price= co.TotalPrice,
                                        co.Company.Name,
                                        co.CompanyId
                                    })
                                
                                }) .FirstOrDefaultAsync();
                                
                if (Product == null)
                {
                    return null;
                }
                var files = await CTX.TbFiles
                    .Where(f => f.EntityId == Product.Id && f.EntityType == tableName.product && !f.IsDeleted)
                    .ToListAsync();
                var result = new ProductModel
                {
                    Product = Product,
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

        public async Task<bool> Add(TbProduct product)
        {
            try
            {
                product.CreatedAt = DateTime.Now;
                product.IsActive = true;
                await CTX.TbProducts.AddAsync(product);
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
                var product = await CTX.TbProducts.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                    return false;

                product.IsDeleted = true;
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
                var LstProduct = await ClsHistory.GetAllHistory(Pageid, id, tableName.product, count);
                if (LstProduct == null)
                {
                    return new List<TbHistory>();

                }
                else
                {
                    return LstProduct;
                }
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }
        public async Task<List<TbProduct>> Search(string name, int page, int count)
        {
            try
            {
                var query = CTX.TbProducts
                    .AsNoTracking()
                    .Where(a =>
                        !a.IsDeleted &&
                        (
                            string.IsNullOrWhiteSpace(name) ||
                            EF.Functions.Like(a.ProductName, $"%{name}%") ||
                            EF.Functions.Like(a.BillingCycle, $"%{name}%") ||
                            a.Price.ToString().Contains(name) ||
                            a.Id.ToString().Contains(name)
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
        public async Task<int> Count()
        {
            try
            {
                var Product = await CTX.TbProducts.AsNoTracking().Where(c => c.IsDeleted == false).CountAsync();
                if (Product == null)
                    return 0;
                return Product;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return 0;
            }
        }
    }
}