using System;
using STMS.Data;
using STMS.Entities;
using STMS.Shared;

namespace STMS.Repos
{
    public class TaskStepRepo(StmsDbContext context) 
    {
        public Result<TaskStep> UpdateStatus(TaskStep model)
        {
            var result = new Result<TaskStep>();
            try
            {
                var objToSave = context.TaskSteps.Find(model.ID);
                if (objToSave == null)
                {
                    objToSave = new TaskStep();
                    context.TaskSteps.Add(objToSave);
                }

                objToSave.TaskID = model.TaskID;
                objToSave.Status = model.Status;
                objToSave.DateTime = DateTime.Now;
                objToSave.PerformedBy = model.PerformedBy; 

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
    }
}