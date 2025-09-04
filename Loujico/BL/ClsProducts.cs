using Loujico.Models;
using Microsoft.EntityFrameworkCore;

namespace Loujico.BL
{
    public interface IProducts
    {
        public Task<List<TbProduct>> GetAllProductsAsync(int id);
        public Task<TbProduct?> GetProductByIdAsync(int id);
        public Task<bool> AddProductAsync(TbProduct product);
        public Task<bool> DeleteAsync(int id);
    }

    public class ClsProducts : IProducts
    {
        CompanySystemContext CTX;
        const int pageSize = 10;

        public ClsProducts(CompanySystemContext companySystemContext)
        {
            CTX = companySystemContext;
        }

        public async Task<List<TbProduct>> GetAllProductsAsync(int id)
        {
            try
            {
                return await CTX.TbProducts
                                .Where(p => p.IsActive).Skip((id - 1) * pageSize)
                                .Take(pageSize)
                                .Include(p => p.TbCustomersProducts)
                                .Include(p => p.TbProductsEmployees)
                                .ToListAsync();
            }
            catch
            {
                return new List<TbProduct>();
            }
        }

        public async Task<TbProduct?> GetProductByIdAsync(int id)
        {
            return await CTX.TbProducts
                            .Include(p => p.TbCustomersProducts)
                            .Include(p => p.TbProductsEmployees)
                            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<bool> AddProductAsync(TbProduct product)
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
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await CTX.TbProducts.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                    return false;

                product.IsActive = false;
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