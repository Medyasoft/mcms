using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Ayyapim.Plugins.ForgotPassword.DataServices;
using Ayyapim.Plugins.ForgotPassword.Helper;
using MCMS.Common;
using MCMS.Common.Extension;
using MCMS.Common.Globalization;
using MCMS.Common.Models.DbModels;
using MCMS.Common.Models.ViewModels;
using MCMS.Common.Result;
using Mcms.Plugins.ForgotPassword.Models;
using Mcms.UI.Framework.PluginArea;
using Mcms.UI.Framework.PluginAttribute;
using Mcms.UI.Framework.Repository;
using Mcms.UI.Framework.Web.Mvc.Result;
using Newtonsoft.Json;

namespace Mcms.Plugins.ForgotPassword.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGeneralSerivce _generalService;
        private readonly IMembershipRepository<Membership> _membershipRepository;

        public HomeController(IGeneralSerivce generalService, IMembershipRepository<Membership> membershipRepository)
        {
            _generalService = generalService;
            _membershipRepository = membershipRepository;
        }

        [ExecutableAction(RouteName = "ForgotPassword")]
        public ActionResult Index(string email)
        {

            Response.ContentType = "application/json";

            OperationResult result = new OperationResult(ResultType.Successfull);
            try
            {
                if (email.IsNull())
                {
                    result.Alerts.Add(new Alert("Lütfen mail adresinizi giriniz".Localize().ToHtmlString(), NotificationType.Error));
                    return new OperatinContentResult(result);
                }

                if (!CommonHelper.IsMailAddress(email) && result.ResultType == ResultType.Successfull)
                {
                    result.Alerts.Add(new Alert("Mail formatınız uygun değil".Localize().ToHtmlString(), NotificationType.Error));
                    return new OperatinContentResult(result);
                }

                if (!_membershipRepository.EmailExists(email) && result.ResultType == ResultType.Successfull)
                {
                    result.Alerts.Add(new Alert("Mail adresiniz sisteme kayıtlı değil".Localize().ToHtmlString(), NotificationType.Error));
                    return new OperatinContentResult(result);
                }


                if (result.ResultType == ResultType.Successfull)
                {
                    var user = _membershipRepository.Select(email);

                    string title = PluginContext.Current.GetProperty("MailTitle");
                    string mailBody = "MailBody".Localize().ToHtmlString();
                    var localization = CallContext.Current.GetObject<LocalizationModel>();

                    string mailLink = string.Format("<a href='{0}?ContentId={1}&Language={2}' target='_blank'>{3}</a>",
                        PluginContext.Current.GetProperty("MailLinkUrl"), user.ContentId, localization.Language,
                        "Şifremi Yenile".Localize().ToHtmlString());
                    mailBody = mailBody + mailLink;

                    var returnValue = MailService.SendMail(MailSetting.Current.Username, user.Email, title, mailBody);
                    if (returnValue != "")
                    {
                        result.Alerts.Add(new Alert(returnValue, NotificationType.Error));
                    }
                    return new OperatinContentResult(result);
                }
                return new OperatinContentResult(result);
            }
            catch (Exception ex)
            {
                result.Alerts.Add(new Alert(ex.FullExceptionMessage(), NotificationType.Error));
                return new JsonStringResult(JsonConvert.SerializeObject(result));
            }
        }

        public string Request(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string responseText = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
            }
            return responseText;
        }

        [System.Web.Mvc.HttpGet]
        [ExecutableAction(RouteName = "ResetPassword")]
        public ActionResult ResetPassword(Guid? contentId)
        {
            if (contentId == null)
                return null;
            var userModel = new UserModel { ContentId = (Guid)contentId };
            return View(userModel);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ResetPassword(UserModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("Confirm", "Şifreler uyumsuz");
                return View(model);
            }

            var member = _membershipRepository.Select(model.ContentId.ToString());

            member.Password = model.Password;
            var result = _membershipRepository.UpdateMember(member);

            if (result.ResultCode != ResultCode.Successfull)
            {
                foreach (var alert in result.Alerts)
                {
                    ModelState.AddModelError("", alert.Message);
                }
            }
            else
            {
                ModelState.AddModelError("Error", "Şifreniz başarıyla değiştirildi".Localize().ToHtmlString());
            }
            return View();
        }
    }
}