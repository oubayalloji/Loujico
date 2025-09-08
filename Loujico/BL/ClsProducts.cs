using Loujico.BL;
using Loujico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Loujico.BL
{
    public interface IProducts
    {
        public Task<List<TbProduct>> GetAllProducts(int id);
        public Task<TbProduct?> GetProductById(int id);
        public Task<bool> Add(TbProduct product);
        public Task<bool> Edit(TbProduct product);
        public Task<bool> Delete(int id);
        public Task<List<TbHistory>> LstEditHistory(int Pageid, int id);
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
        public async Task<List<TbProduct>> GetAllProducts(int id)
        {
            try
            {
                return await CTX.TbProducts
                                .Where(p => p.IsActive).Skip((id - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }

        public async Task<TbProduct?> GetProductById(int id)
        {
            try
            {
                return await CTX.TbProducts
                                .Include(p => p.TbCustomersProducts)
                                .Include(p => p.TbProductsEmployees)
                                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
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
                if (string.IsNullOrEmpty(product.ProductName))
                    return false;

                if (string.IsNullOrEmpty(product.ProductDescription))
                    return false;

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
        public async Task<List<TbHistory>> LstEditHistory(int Pageid, int id)
        {
            try
            {
                var LstProduct = await ClsHistory.GetAllHistory(Pageid, id, "TbProduct");
                if (LstProduct != null)
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
                return new List<TbHistory>();
            }
        }
    }
}