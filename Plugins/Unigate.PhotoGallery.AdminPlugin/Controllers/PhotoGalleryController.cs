using Mcms.Sdk;
using Mcms.UI.Framework;
using Mcms.UI.Framework.Authorization;
using Mcms.UI.Framework.PluginAttribute;
using MCMS.Common.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mcms.UI.Framework.Web.Mvc.Routing;
using System.Net;

namespace Unigate.PhotoGallery.AdminPlugin.Controllers
{
    [AdminController(Name = "Photo Gallery Controller")]
    [Authorization(Role = "Admin")]
    public class PhotoGalleryController : Controller
    {
        public string[] RouteValues
        {
            get
            {
                string[] retVal = null;

                try
                {
                    var routeValue = RouteData.GetRouteValue("RouteValue");
                    if (routeValue != null)
                    {
                        if (routeValue.Contains("/"))
                            retVal = routeValue.Split('/').ToArray().Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
                        else
                            retVal = new[] { routeValue };
                    }
                }
                catch (Exception)
                { }

                return retVal;
            }
        }

        public string LastRouteValue
        {
            get
            {
                string retVal = string.Empty;

                string[] routeValues = this.RouteValues;
                if (routeValues != null && routeValues.Length > 0)
                    retVal = routeValues.Last();

                return retVal;
            }
        }

        int? albumListId = null;
        public int AlbumListId
        {
            get
            {
                if (!this.albumListId.HasValue)
                    this.albumListId = UnigateObject.Query("List").Where("TableName", Manager.Settings.Current.AlbumListTableName).FirstOrDefault<MCMS.Common.Models.DbModels.MList>().Id;

                return this.albumListId.Value;
            }
        }

        int? photoListId = null;
        public int PhotoListId
        {
            get
            {
                if (!this.photoListId.HasValue)
                    this.photoListId = UnigateObject.Query("List").Where("TableName", Manager.Settings.Current.PhotoListTableName).FirstOrDefault<MCMS.Common.Models.DbModels.MList>().Id;

                return this.photoListId.Value;
            }
        }

        public ActionResult Album()
        {
            return Redirect(string.Format("/unigate/ListItem/Index/{0}", this.AlbumListId));
        }

        [HttpGet]
        public ActionResult Index(Guid? albumId)
        {
            ViewBag.Albums = UnigateObject.Query(Manager.Settings.Current.AlbumListTableName).Where("SiteLanguageId", MCMS.Common.SettingContext.Current.DefaultSiteLanguageId).SortAsc("Title").ToList<Models.BaseModel>();
            ViewBag.SelectedAlbumId = string.Empty;

            List<Models.PhotoViewModel> viewModel = null;

            if (albumId.HasValue && albumId.Value != Guid.Empty)
            {
                ViewBag.SelectedAlbumId = albumId.ToString();

                viewModel = this.GetPhotosByAlbumId(albumId.Value);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            Guid albumId = Guid.Empty;
            if (form["Album-Select"] != null && Guid.TryParse(form["Album-Select"].ToString(), out albumId) && albumId != Guid.Empty)
            {
                return this.Index(albumId);
            }
            else
                return this.Index(Guid.Empty);
        }

        [HttpPost]
        public JsonResult Upload(Guid albumId)
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                int orderno = 1;
                if (Manager.Settings.Current.PhotoListTableOrderNoField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableOrderNoField.Name))
                {
                    orderno = UnigateObject.Query(Manager.Settings.Current.PhotoListTableName).ToList<Models.BaseModel>().Count + 1;
                }

                string photofolderpath = Server.MapPath(string.Format("~/documents/file/{0}", Manager.Settings.Current.PhotoListTableName));

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    string filename = Path.GetFileName(Request.Files[i].FileName);

                    Stream stream = Request.Files[i].InputStream;
                    byte[] photobinarydata = new byte[stream.Length];
                    stream.Read(photobinarydata, 0, Convert.ToInt32(stream.Length));
                    Size imagesize = Manager.PhotoManager.GetImageSize(photobinarydata);

                    this.ChangeFileFieldType(false);
                    Insert insert = UnigateObject.Insert(Manager.Settings.Current.PhotoListTableName)
                                                 .Column("Slug", filename.Replace(".", "-"))
                                                 .Column("CreateDate", DateTime.Now.ToString())
                                                 .Column("UpdateDate", DateTime.Now.ToString());

                    if (Manager.Settings.Current.PhotoListTableTitleField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableTitleField.Name))
                    {
                        if (Manager.Settings.Current.PhotoListTableTitleField.DefaultValue != null)
                        {
                            switch (Manager.Settings.Current.PhotoListTableTitleField.DefaultValue.ToString())
                            {
                                case "FileName":
                                    insert = insert.Column(Manager.Settings.Current.PhotoListTableTitleField.Name, filename);
                                    break;
                                case "Now":
                                    insert = insert.Column(Manager.Settings.Current.PhotoListTableTitleField.Name, DateTime.Now.ToString());
                                    break;
                                case "Today":
                                    insert = insert.Column(Manager.Settings.Current.PhotoListTableTitleField.Name, DateTime.Today.ToShortDateString());
                                    break;
                                default:
                                    insert = insert.Column(Manager.Settings.Current.PhotoListTableTitleField.Name, string.Empty);
                                    break;
                            }
                        }
                        else
                        {
                            insert = insert.Column(Manager.Settings.Current.PhotoListTableTitleField.Name, string.Empty);
                        }
                    }

                    bool uploaded = false;
                    string uploadfilename = Manager.PhotoManager.GetUnigateFileName(filename);
                    if (Manager.Settings.Current.PhotoListTablePhotoField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTablePhotoField.Name))
                    {
                        string uploadpath = string.Format("{0}/{1}", photofolderpath, uploadfilename);

                        if (Manager.Settings.Current.PhotoSize != null && Manager.Settings.Current.PhotoSize.Width != null && Manager.Settings.Current.PhotoSize.Height != null)
                        {
                            uploaded = Manager.PhotoManager.SavePhoto(Manager.PhotoManager.ImageResize(photobinarydata, Manager.Settings.Current.PhotoSize.Width, Manager.Settings.Current.PhotoSize.Height, false), uploadpath);
                        }
                        else
                        {
                            uploaded = Manager.PhotoManager.SavePhoto(photobinarydata, uploadpath);
                        }

                        insert = insert.Column(Manager.Settings.Current.PhotoListTablePhotoField.Name, string.Format("/documents/file/{0}/{1}", Manager.Settings.Current.PhotoListTableName, uploadfilename));
                    }

                    if (uploaded)
                    {
                        if (Manager.Settings.Current.PhotoListTableThumbnailField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableThumbnailField.Name))
                        {
                            string uploadthumbnailname = Manager.PhotoManager.GetUnigateFileName(filename);
                            string uploadthumbnailpath = string.Format("{0}/{1}", photofolderpath, uploadthumbnailname);

                            if (Manager.Settings.Current.ThumbnailPhotoSize != null && Manager.Settings.Current.ThumbnailPhotoSize.Width != null && Manager.Settings.Current.ThumbnailPhotoSize.Height != null)
                            {
                                Manager.PhotoManager.SavePhoto(Manager.PhotoManager.ImageResize(photobinarydata, Manager.Settings.Current.ThumbnailPhotoSize.Width, Manager.Settings.Current.ThumbnailPhotoSize.Height, false), uploadthumbnailpath);
                            }
                            else
                            {
                                uploadthumbnailname = uploadfilename;
                            }

                            insert = insert.Column(Manager.Settings.Current.PhotoListTableThumbnailField.Name, string.Format("/documents/file/{0}/{1}", Manager.Settings.Current.PhotoListTableName, uploadthumbnailname));
                        }

                        if (Manager.Settings.Current.PhotoListTableAlbumRelationField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableAlbumRelationField.Name))
                        {
                            insert = insert.Column(Manager.Settings.Current.PhotoListTableAlbumRelationField.Name, albumId);
                        }

                        if (Manager.Settings.Current.PhotoListTableOrderNoField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableOrderNoField.Name))
                        {
                            insert = insert.Column(Manager.Settings.Current.PhotoListTableOrderNoField.Name, orderno);
                            orderno++;
                        }

                        MDataSourceResult result = insert.Execute();
                        this.ChangeFileFieldType(true);
                    }
                }
            }

            return Json("/unigate/" + Unigate.PhotoGallery.AdminPlugin.Manager.PluginProperties.PluginName + "/?albumId=" + albumId.ToString());
        }

        public ActionResult SaveChanges(FormCollection form)
        {
            Guid albumId = Guid.Parse(form["hdAlbumId"].ToString());

            List<Models.PhotoViewModel> photos = this.GetPhotosByAlbumId(albumId);
            if (photos != null && photos.Count > 0)
            {
                foreach (var item in photos)
                {
                    Update update = UnigateObject.Update(Manager.Settings.Current.PhotoListTableName);

                    if (Manager.Settings.Current.PhotoListTableTitleField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableTitleField.Name))
                        update = update.Column(Manager.Settings.Current.PhotoListTableTitleField.Name, form["Txt_" + item.Id.ToString()].ToString());

                    if (Manager.Settings.Current.PhotoListTableOrderNoField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableOrderNoField.Name))
                        update = update.Column(Manager.Settings.Current.PhotoListTableOrderNoField.Name, Convert.ToInt32(form["Hd_" + item.Id.ToString()].ToString()));

                    update.Where("Id", item.Id).Execute();
                }
            }

            return Redirect("/unigate/" + Unigate.PhotoGallery.AdminPlugin.Manager.PluginProperties.PluginName + "/?albumId=" + albumId.ToString());
        }

        [ExecutableAction(RouteName = "PhotoRemove")]
        public ActionResult Remove(string photoId)
        {
            MDataSourceResult result = UnigateObject.Delete(Manager.Settings.Current.PhotoListTableName).Where("Id", Convert.ToInt32(photoId)).Execute();
            if (result.ResultCode == MCMS.Common.Result.ResultCode.Successfull)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        List<Models.PhotoViewModel> GetPhotosByAlbumId(Guid albumId)
        {
            Query query = UnigateObject.Query(Manager.Settings.Current.PhotoListTableName)
                                       .Where(Manager.Settings.Current.PhotoListTableAlbumRelationField.Name, albumId.ToString());

            if(Manager.Settings.Current.PhotoListTableOrderNoField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableOrderNoField.Name))
            {
                query = query.SortAsc(Manager.Settings.Current.PhotoListTableOrderNoField.Name);
            }
            else
            {
                query = query.SortDesc("CreateDate");
            }

            DataTable photos = query.ToDataTable();

            List<Models.PhotoViewModel> retVal = null;
            if (photos != null && photos.Rows.Count > 0)
            {
                retVal = photos.AsEnumerable().Select(p => new Models.PhotoViewModel()
                {
                    ContentId = p.Field<Guid>("ContentId"),
                    Id = p.Field<int>("Id"),
                    Photo = p.Field<string>(Manager.Settings.Current.PhotoListTablePhotoField.Name),
                    SiteLanguageId = p.Field<Guid>("SiteLanguageId"),
                    Slug = p.Field<string>("Slug"),
                    Title = (Manager.Settings.Current.PhotoListTableTitleField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableTitleField.Name) && !p.IsNull(Manager.Settings.Current.PhotoListTableTitleField.Name)) ? p.Field<string>(Manager.Settings.Current.PhotoListTableTitleField.Name) : string.Empty,
                    OrderNo = (Manager.Settings.Current.PhotoListTableOrderNoField != null && !string.IsNullOrEmpty(Manager.Settings.Current.PhotoListTableOrderNoField.Name) && !p.IsNull(Manager.Settings.Current.PhotoListTableOrderNoField.Name)) ? p.Field<int>(Manager.Settings.Current.PhotoListTableOrderNoField.Name) : 0
                }).ToList();
            }

            return retVal;
        }

        void ChangeFileFieldType(bool isfiletype)
        {
            DataEngine de = new DataEngine(ConfigurationManager.ConnectionStrings[0].ConnectionString);

            if (Manager.Settings.Current.PhotoListTablePhotoField != null)
            {
                de.ExecuteNonQuery("UPDATE ListColumn SET [Type] = @type WHERE ListId = @id AND FieldName = @name", new SqlParameter("@type", (isfiletype ? 26 : 1)), new SqlParameter("@id", this.PhotoListId), new SqlParameter("@name", Manager.Settings.Current.PhotoListTablePhotoField.Name));
            }

            if (Manager.Settings.Current.PhotoListTableThumbnailField != null)
            {
                de.ExecuteNonQuery("UPDATE ListColumn SET [Type] = @type WHERE ListId = @id AND FieldName = @name", new SqlParameter("@type", (isfiletype ? 26 : 1)), new SqlParameter("@id", this.PhotoListId), new SqlParameter("@name", Manager.Settings.Current.PhotoListTableThumbnailField.Name));
            }
        }

        public ActionResult Flickr()
        {
            ViewBag.Albums = UnigateObject.Query(Manager.Settings.Current.AlbumListTableName).Where("SiteLanguageId", MCMS.Common.SettingContext.Current.DefaultSiteLanguageId).SortAsc("Title").ToList<Models.BaseModel>();
            ViewBag.SelectedAlbumId = string.Empty;

            return View(this.GetPhotosFromFlickr());
        }

        [HttpPost]
        [ExecutableAction(RouteName = "GetFlickrPhotoPerPage")]
        public JsonResult GetFlickrPhotoPerPage(int pageIndex)
        {
            return Json(this.GetPhotosFromFlickr(pageIndex));
        }

        List<Models.FlickrPhoto> GetPhotosFromFlickr(int pageindex = 0)
        {
            List<Models.FlickrPhoto> retVal = null;

            FlickrNet.Flickr flickr = new FlickrNet.Flickr(Manager.Settings.Current.FlickrAPIKey);
            flickr.InstanceCacheDisabled = true;

            FlickrNet.FoundUser userInfo = flickr.PeopleFindByUserName(Manager.Settings.Current.FlickrUserName);
            string flickrUserId = userInfo.UserId;
            ViewBag.FlickrFullName = userInfo.FullName;

            FlickrNet.Person user = flickr.PeopleGetInfo(flickrUserId);
            ViewBag.FlickrFullName = user.RealName;

            FlickrNet.PhotoSearchOptions option = new FlickrNet.PhotoSearchOptions();
            option.UserId = flickrUserId;
            option.Page = pageindex;

            FlickrNet.PhotoCollection photosets = flickr.PhotosSearch(option);
            if (photosets != null && photosets.Count > 0)
            {
                DataTable photos = UnigateObject.Query(Manager.Settings.Current.PhotoListTableName)
                                                .Column("Slug")
                                                .SortDesc("CreateDate")
                                                .ToDataTable();

                List<string> slugs = new List<string>();
                if (photos != null && photos.Rows.Count > 0)
                {
                    slugs = photos.AsEnumerable().Select(p => p.Field<string>("Slug")).ToList();
                }

                retVal = photosets.Where(p => !slugs.Any(s => s == p.PhotoId)).Select(p => new Models.FlickrPhoto()
                {
                    DownloadUrl = !string.IsNullOrEmpty(p.LargeUrl) ? p.LargeUrl : (!string.IsNullOrEmpty(p.MediumUrl) ? p.MediumUrl : p.SmallUrl),
                    ViewUrl = p.SmallUrl,
                    Title = p.Title,
                    PhotoId = p.PhotoId
                }).ToList<Models.FlickrPhoto>();
            }

            return retVal;
        }

        public ActionResult MoveFlickrPhotos(FormCollection form)
        {
            string photofolderpath = Server.MapPath(string.Format("~/documents/file/{0}", Manager.Settings.Current.PhotoListTableName));

            Guid albumId = Guid.Parse(form["Album-Select"].ToString());

            foreach (var item in form.AllKeys)
            {
                if (item.StartsWith("hd_"))
                {
                    string photoid = item.Substring(3);
                    if (form["chc_" + photoid] == "on")
                    {
                        string downloadlink = form["hd_" + photoid].ToString();
                        string thumbnaillink = form["hdt_" + photoid].ToString();
                        string title = form["Txt_" + photoid].ToString();
                        string[] linksegments = downloadlink.Split('/');
                        string uploadfilename = Manager.PhotoManager.GetUnigateFileName(linksegments.Last());

                        this.ChangeFileFieldType(false);
                        Insert insert = UnigateObject.Insert(Manager.Settings.Current.PhotoListTableName)
                                                 .Column("Slug", photoid)
                                                 .Column("CreateDate", DateTime.Now.ToString())
                                                 .Column("UpdateDate", DateTime.Now.ToString());

                        if (Manager.Settings.Current.PhotoListTableTitleField != null)
                        {
                            insert = insert.Column(Manager.Settings.Current.PhotoListTableTitleField.Name, title);
                        }

                        bool uploaded = false;
                        if (Manager.Settings.Current.PhotoListTablePhotoField != null)
                        {
                            string uploadpath = string.Format("{0}/{1}", photofolderpath, uploadfilename);
                            using (WebClient client = new WebClient())
                            {
                                try
                                {
                                    client.DownloadFile(downloadlink, uploadpath);
                                    uploaded = true;

                                    insert = insert.Column(Manager.Settings.Current.PhotoListTablePhotoField.Name, string.Format("/documents/file/{0}/{1}", Manager.Settings.Current.PhotoListTableName, uploadfilename));
                                }
                                catch (Exception)
                                {
                                    uploaded = false;
                                }
                            }
                        }

                        if (uploaded)
                        {
                            if (Manager.Settings.Current.PhotoListTableThumbnailField != null)
                            {
                                string uploadthumbnailname = Manager.PhotoManager.GetUnigateFileName(linksegments.Last());
                                string uploadthumbnailpath = string.Format("{0}/{1}", photofolderpath, uploadthumbnailname);
                                using (WebClient client = new WebClient())
                                {
                                    try
                                    {
                                        client.DownloadFile(downloadlink, uploadthumbnailpath);

                                        insert = insert.Column(Manager.Settings.Current.PhotoListTableThumbnailField.Name, string.Format("/documents/file/{0}/{1}", Manager.Settings.Current.PhotoListTableName, uploadthumbnailname));
                                    }
                                    catch (Exception)
                                    { }
                                }
                            }

                            if (Manager.Settings.Current.PhotoListTableAlbumRelationField != null)
                            {
                                insert = insert.Column(Manager.Settings.Current.PhotoListTableAlbumRelationField.Name, albumId);
                            }

                            MDataSourceResult result = insert.Execute();
                            
                        }
                        this.ChangeFileFieldType(true);
                    }
                }
            }

            return Redirect("/unigate/" + Unigate.PhotoGallery.AdminPlugin.Manager.PluginProperties.PluginName + "/PhotoGallery/Flickr");
        }
    }
}