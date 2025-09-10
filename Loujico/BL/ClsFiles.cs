using Loujico.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Loujico.BL
{
    public interface IFiles
    {
        public  Task<string> UploadImage(List<IFormFile> files, string folderName);

        public  Task<bool> Add(FileModel file, string folderPath, int Id, string EntityType);
        public Task<bool> Delete(int id, string EntityType);
        public Task<TbFile> GetById(int id, string EntityType);
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

        public async Task<bool> Add(FileModel file, string folderPath, int Id , string EntityType)
        {
            try
            {
                TbFile tbFile = new TbFile() ;
                string doc = await UploadImage(file.Files, folderPath);
              
                tbFile.EntityId = Id; 
                tbFile.EntityType=EntityType; 
                
                tbFile.FileName = doc;
                tbFile.FileType = file.fileType;
                tbFile.UploadedAt = DateTime.Now;


                await CTX.TbFiles.AddAsync(tbFile);
                await CTX.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return false;
            }
        }

        public async Task<bool> Delete(int id, string EntityType)
        {
            try
            {
                var file = await CTX.TbFiles.FirstOrDefaultAsync(f => f.EntityId == id && f.EntityType==EntityType);
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

        public async Task<TbFile> GetById(int id, string EntityType)
        {
            try
            {
                var file = await CTX.TbFiles.FirstOrDefaultAsync(f => f.EntityId == id && f.EntityType == EntityType);
                if (file == null)
                    return null;
                return file;
            }
            catch (Exception ex)
            {
                await ClsLogs.Add("Error", ex.Message, null);
                return null;
            }
        }

        public async Task<string> UploadImage(List<IFormFile> files,string folderName)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Upload", folderName, imageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return imageName;
                }
            }
            return "";
        }
      /*  public async Task<string> DeleteImageAsync(string imageName)
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
        }*/
    }
}



