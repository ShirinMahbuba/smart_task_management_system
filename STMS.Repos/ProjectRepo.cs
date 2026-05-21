using STMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STMS.Entities;
using STMS.Shared;

namespace STMS.Repos
{
    public class ProjectRepo(StmsDbContext context)
    { 
        public Result<List<Project>> GetAll()
        {
            var result = new Result<List<Project>>();
            try
            {
                result.Data = context.Projects
                    .Include(e=> e.Tasks)
                    .Include(e=> e.Creator)
                    .ToList();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public Result<Project> GetById(int id)
        {
            var result = new Result<Project>();
            try
            {
                result.Data = context.Projects
                    .Include(e => e.Tasks)
                    .Include(e => e.Creator)
                    .FirstOrDefault(e=> e.ID == id);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public Result<Project> Delete(int id)
        {
            var result = new Result<Project>();
            try
            {
                var project = context.Projects.Find(id);
                if (project != null)
                {
                    context.Projects.Remove(project);
                    context.SaveChanges();
                    result.Data = project;
                }
                else
                {
                    result.HasError = true;
                    result.Message = "Project not found.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public Result<Project> Save(Project project)
        {
            var result = new Result<Project>();
            try
            {
                if (context.Projects.Any(p => p.ProjectName == project.ProjectName && p.ID != project.ID))
                {
                    result.HasError = true;
                    result.Message = "Project with the same name already exists.";
                    return result;
                }

                var objToSave = context.Projects.Find(project.ID);
                if (objToSave != null)
                {
                    objToSave.ProjectName = project.ProjectName;
                    objToSave.Description = project.Description;
                    objToSave.StartDate = project.StartDate;
                    objToSave.EndDate = project.EndDate;
                    objToSave.UpdatedAt = DateTime.Now;
                    objToSave.UpdatedBy = 1;
                    context.Projects.Update(objToSave);
                }
                else
                {
                    objToSave = new Project
                    {
                        ProjectName = project.ProjectName,
                        Description = project.Description,
                        CreatedBy = 1,
                        StartDate = project.StartDate,
                        EndDate = project.EndDate,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = 1
                    };
                    context.Projects.Add(objToSave);
                }

                context.SaveChanges();
                result.Data = objToSave;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
