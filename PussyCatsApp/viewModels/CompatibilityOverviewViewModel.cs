using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Services;

namespace PussyCatsApp.ViewModels
{
    public class CompatibilityOverviewViewModel : INotifyPropertyChanged
    {
        private List<RoleResult> roleResults;
        private RoleResult selectedResult;
        private string errorMessage;
        private int currentUserId;
        private ICompatibilityService compatibilityService;

        public event PropertyChangedEventHandler PropertyChanged;

        public CompatibilityOverviewViewModel(int userId, ICompatibilityService compatibilityService)
        {
            this.compatibilityService = compatibilityService;
            this.currentUserId = userId;
            this.roleResults = new List<RoleResult>();
        }

        public void LoadAllRoles()
        {
            try
            {
                roleResults = compatibilityService.CalculateAll(currentUserId);
                errorMessage = null;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public List<RoleResult> GetRoleResults()
        {
            return roleResults;
        }

        public RoleResult GetResultForRole(JobRole role)
        {
            foreach (RoleResult result in roleResults)
            {
                if (result.JobRole == role)
                {
                    return result;
                }
            }
            return null;
        }

        public void OnRoleSelected(JobRole role)
        {
            selectedResult = GetResultForRole(role);
        }

        public RoleResult GetSelectedResult()
        {
            return selectedResult;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }
}
