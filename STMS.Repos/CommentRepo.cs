using STMS.Data;
using STMS.Entities;
using STMS.Shared;

namespace STMS.Repos
{
    public class CommentRepo(StmsDbContext context)
    {
        public Result<List<Comment>> GetByTaskId(int taskId)
        {
            var result = new Result<List<Comment>>();
            try
            {
                result.Data = context.Comments
                                     .Where(c => c.TaskID == taskId)
                                     .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<Comment> GetById(int id)
        {
            var result = new Result<Comment>();
            try
            {
                result.Data = context.Comments.Find(id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<Comment> Save(Comment model)
        {
            var result = new Result<Comment>();
            try
            {
                var objToSave = context.Comments.Find(model.ID);
                if (objToSave == null)
                {
                    objToSave = new Comment();
                    objToSave.CreatedAt = DateTime.Now;   
                    context.Comments.Add(objToSave);
                }

                objToSave.TaskID = model.TaskID;
                objToSave.UserID = model.UserID;
                objToSave.CommentText = model.CommentText;
                objToSave.UpdatedAt = DateTime.Now;
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

        public Result<Comment> Delete(int id)
        {
            var result = new Result<Comment>();
            try
            {
                var objToDelete = context.Comments.Find(id);
                if (objToDelete == null)
                {
                    result.HasError = true;
                    result.Message = "Comment Not Found";
                    return result;
                }

                context.Comments.Remove(objToDelete);
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