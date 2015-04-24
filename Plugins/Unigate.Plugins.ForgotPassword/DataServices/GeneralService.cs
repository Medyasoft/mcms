using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCMS.Common.Models.ViewModels;
using Mcms.Plugins.ForgotPassword.Models;
using Mcms.Sdk;

namespace Ayyapim.Plugins.ForgotPassword.DataServices
{
    public class GeneralService : IGeneralSerivce
    {
        public bool CheckUser(string mailAddress)
        {
            return GetUserByMail(mailAddress) != null;
        }


        public UserModel GetUserByMail(string mailAddress)
        {
            var user = UnigateObject.Query("ayMembers")
                .Where("Email", mailAddress)
                .ToList<UserModel>()
                .FirstOrDefault();
            return user;
        }

        public UserModel GetUserByContentId(Guid contentId)
        {
            var user = UnigateObject.Query("ayMembers")
                  .Where("ContentId", contentId)
                  .ToList<UserModel>()
                  .FirstOrDefault();
            return user;
        }

        public MDataSourceResult UpdateUserPassword(UserModel model)
        {
            return UnigateObject.Update("ayMembers")
                .Column("Password", model.Password)
                .Column("Title", model.Title)
                .Where("ContentId", model.ContentId).Execute();
        }
    }
}