using STMS.Data;
using STMS.Shared;
using AttachmentEntity = STMS.Entities.Attachment;

namespace STMS.Repos
{
    public class AttachmentRepo(StmsDbContext context)
    {
        public Result<List<AttachmentEntity>> GetByTaskId(int taskId)
        {
            var result = new Result<List<AttachmentEntity>>();
            try
            {
                result.Data = context.Attachments
                                     .Where(a => a.TaskID == taskId)
                                     .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<AttachmentEntity> Save(AttachmentEntity model)
        {
            var result = new Result<AttachmentEntity>();
            try
            {
                var objToSave = new AttachmentEntity();
                context.Attachments.Add(objToSave);

                objToSave.TaskID = model.TaskID;
                objToSave.UploadedBy = model.UploadedBy;
                objToSave.FileName = model.FileName;
                objToSave.FilePath = model.FilePath;
                objToSave.UpdatedAt = DateTime.Now;
                objToSave.UpdatedBy = model.UpdatedBy;
                context.SaveChanges();
                result.Data = objToSave;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<AttachmentEntity> Delete(int id)
        {
            var result = new Result<AttachmentEntity>();
            try
            {
                var objToDelete = context.Attachments.Find(id);
                if (objToDelete == null)
                {
                    result.HasError = true;
                    result.Message = "Attachment Not Found";
                    return result;
                }

                context.Attachments.Remove(objToDelete);
                context.SaveChanges();
                result.Data = objToDelete;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }
    }
}