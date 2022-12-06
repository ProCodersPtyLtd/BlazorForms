using BlazorForms.Platform.Crm.Business.Admin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Crm.Definitions.Services
{
    public interface IBackgroundTaskService
    {
        Task EnqueueTask(Func<Task> workItem, string name, string type = "", Func<Task> onCompletedAsync = null);
        Task<List<BackgroundTaskModel>> GetTasks();
        Task<BackgroundTaskModel> GetTaskById(int id);
        Task<int> SaveSystemTask(BackgroundTaskModel data);
        Task UpdateSystemTask(int id, string details, string state, int duration);
        Task DeleteSystemTask(int id);
        Task<List<BackgroundTaskModel>> GetTasksByName(string name);
        Task<BackgroundTaskModel> GetLastTaskByType(string type);
    }
}
