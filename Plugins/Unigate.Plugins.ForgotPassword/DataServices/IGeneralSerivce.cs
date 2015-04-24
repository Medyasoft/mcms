using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCMS.Common.Models.ViewModels;
using Mcms.Plugins.ForgotPassword.Models;

namespace Ayyapim.Plugins.ForgotPassword.DataServices
{
    public interface IGeneralSerivce
    {
        bool CheckUser(string mailAddress);

        UserModel GetUserByMail(string mailAddress);
        UserModel GetUserByContentId(Guid contentId);
        MDataSourceResult UpdateUserPassword(UserModel model);
    }
}