using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Loujico.BL
{
    public interface IFiles
    {
        public Task<bool> Add(IFormFile uploadedFile, TbFile fileMeta, string folderPath);
        public Task<bool> Delete(int id);
    }

    public class ClsFiles : IFiles
    {
        CompanySystemContext CTX;
        Ilog ClsLogs;

        public ClsFiles(CompanySystemContext companySystemContext, Ilog clsLogs)
        {
            CTX = companySystemContext;
            ClsLogs = clsLogs;
        }

        public async Task<bool> Add(IFormFile uploadedFile, TbFile fileMeta, string folderPath)
        {
            try
            {
                // 1. توليد اسم فريد للملف
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                // 2. حفظ الملف على السيرفر
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                // 3. حفظ البيانات الوصفية في قاعدة البيانات
                fileMeta.FileName = fileName;
                fileMeta.FileType = uploadedFile.ContentType;
                fileMeta.UploadedAt = DateTime.Now;
                fileMeta.IsDeleted = false;

                await CTX.TbFiles.AddAsync(fileMeta);
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
                var file = await CTX.TbFiles.FirstOrDefaultAsync(f => f.Id == id);
                if (file == null)
                    return false;

                file.IsDeleted = true;
                CTX.Entry(file).State = EntityState.Modified;
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }
        public async Task<string> UploadImage(List<IFormFile> files,string folderName)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName, imageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return imageName;
                }
            }
            return "";
        }
        public async Task<string> DeleteImageAsync(string imageName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageName))
                {
                    return "اسم الصورة غير صالح";
                }


                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload");
                string filePath = Path.Combine(uploadsFolder, imageName);

                if (!File.Exists(filePath))
                {
                    return "الصورة غير موجودة على السيرفر";
                }

                await Task.Run(() => File.Delete(filePath));
                return null;
            }   
            catch (IOException ioEx)
            {
                return $"خطأ في النظام: {ioEx.Message}";
            }
            catch (Exception ex)
            {
                return $"حدث خطأ غير متوقع: {ex.Message}";
            }
        }
    }
}



