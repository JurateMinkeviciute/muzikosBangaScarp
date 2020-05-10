using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Diagnostics;
using MuzikosBangaCsharp.Models;
using System.Configuration;
using MuzikosBangaCsharp.ViewModels;
using System.Web.Security;

namespace MuzikosBangaCsharp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        DbConnection dbcon = new DbConnection();
        string messageLogin = null;

        [Authorize]
        public ActionResult Index()
        {
            List<Album> ListOfAlbum = dbcon.getAlbumsFromDb(QueryConstants.queryGet10Albums);
            return View(ListOfAlbum);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.Message = messageLogin;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = dbcon.checkLogin(model, QueryConstants.queryCheckLogin);
                if (user.username == null)
                {
                    ModelState.AddModelError(string.Empty, "Prisijungimas negalimas");
                    return View(model);
                }

                Session["id"] = user.id;
                Session["username"] = user.username;
                FormsAuthentication.SetAuthCookie(user.username, false);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = dbcon.userRegistration(model, QueryConstants.queryUserRegistration);
                if (success)
                {
                    ModelState.Clear();
                    messageLogin = "Registracija sėkminga, dabar galite prisijungti. <br/><a href='/Login'>Prisijungimas čia.</a>";
                    return View();
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-10);
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public PartialViewResult Search(string search = "", bool isPartial = false)
        {
            var searchModel = getModelForSearchPage(search);
            var username = (string)Session["username"];
            searchModel.dropdownList = dbcon.getPlaylistsDropdown(username);
            return isPartial ? PartialView("_Search", searchModel) : PartialView(searchModel);
        }

        [Authorize]
        public PartialViewResult Browse(bool isPartial = false)
        {
            List<Album> ListOfAlbum = dbcon.getAlbumsFromDb(QueryConstants.queryGet10Albums);
            return isPartial ? PartialView("_Browse", ListOfAlbum) : PartialView(ListOfAlbum);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult Artist(int id, bool isPartial = false)
        {
            var username = (string)Session["username"];
            ArtistViewModel artistWithSong = dbcon.getArtist(id);
            foreach(var song in artistWithSong.songs)
            {
                artistWithSong.songsIds.Add(song.id);
            }
            return isPartial ? PartialView("_Artist", artistWithSong) : PartialView(artistWithSong);
        }

        [Authorize]
        public PartialViewResult Album(int id, bool isPartial = false)
        {
            var username = (string)Session["username"];
            AlbumViewModel albumModel = new AlbumViewModel();
            albumModel = dbcon.getAlbum(id);
            albumModel.NumberOfSongs = albumModel.songs.Count();
            foreach(var SongId in albumModel.songs)
            {
                albumModel.songsIds.Add(SongId.id);
            }

            albumModel.dropdownList = dbcon.getPlaylistsDropdown(username);
            return isPartial ? PartialView("_Album", albumModel) : PartialView(albumModel);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult YourMusic(bool isPartial = false)
        {
            var username = (string)Session["username"];
            List<Playlist> playlist = dbcon.getYourMusic(username);
            return isPartial ? PartialView("_YourMusic", playlist) : PartialView(playlist);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult Playlist(int id, bool isPartial = false)
        {
            string username = (string)Session["username"];
            var playlistSongs = dbcon.getPlaylistSongs(id, username);
            playlistSongs.numberOfSongs = playlistSongs.songs.Count();
            foreach (var song in playlistSongs.songs)
            {
                playlistSongs.songsIds.Add(song.id);
            }
            return isPartial ? PartialView("_Playlist", playlistSongs) : PartialView(playlistSongs);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult Settings(bool isPartial = false)
        {
            return isPartial ? PartialView("_Settings") : PartialView();
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult UpdateDetails(bool isPartial = false)
        {
            int id = (int)Session["id"];
            UpdateEmailViewModel emailModel = dbcon.getUserEmail(id);

            return isPartial ? PartialView("_UpdateDetails") : PartialView();
        }

        // ** ** ** Ajax Calls and Functions ** ** ** //

        // For Search Action ajax call
        [HttpGet]
        [AllowAnonymous]
        public ActionResult searchAjax(string search = "")
        {
            var searchModel = getModelForSearchPage(search);
            return PartialView("_SearchResult", searchModel);
        }
        [HttpGet]
        [Authorize]
        public SearchViewModel getModelForSearchPage(string search)
        {
            SearchViewModel searchModel = new SearchViewModel()
            {
                search = search
            };

            if (search == "")
                return searchModel;

            searchModel = dbcon.getSearchSongs(search, searchModel);
            searchModel = dbcon.getSearchArtists(search, searchModel);
            searchModel = dbcon.getSearchAlbums(search, searchModel);

            return searchModel;
        }
        // User have his own playlist list 
        [HttpGet]
        [Authorize]
        public List<DropdownViewModel> PlaylistsDropdown()
        {
            var username = (string)Session["username"];
            var dropdownViewModels = dbcon.getPlaylistsDropdown(username);

            return dropdownViewModels;
        }
        // For first page random 10 songs
        public JsonResult getRandom10Songs()
        {
            var random10songs = dbcon.getRandom10Songs();
            return Json(random10songs, JsonRequestBehavior.AllowGet);
        }

        // Ajax functions
        public JsonResult addToPlaylist(int playlistId, int songId)
        {
            if (dbcon.addToPlaylist(playlistId, songId))
            {
                return Json("Added successful", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult createPlaylist(string name)
        {
            string username = (string)Session["username"];
            DateTime dateTime = DateTime.Now;

            if (dbcon.createPlaylist(name, username, dateTime))
            {
                return Json("Added successful", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult deletePlaylist(int playlistId)
        {
            if (dbcon.deletePlaylist(playlistId))
            {
                return Json("delete successful", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getAlbumJson(int albumId)
        {
            var album = dbcon.getAlbumJson(albumId);
            return Json(album, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getArtistJson(int artistId)
        {
            var artist = dbcon.getArtistJson(artistId);
            return Json(artist, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getSongJson(int songId)
        {
            var song = dbcon.getSongJson(songId);
            return Json(song, JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeFromPlaylist(int playlistId, int songId)
        {
            if (dbcon.removeFromPlaylist(playlistId, songId))
            {
                return Json("remove from playlist successful", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public string UpdateEmail(string email)
        {
            var message = "";
            var id = (int)Session["id"];

            if (email == "" || email == null)
            {
                message = "You must provide an email";
                return message;
            }

            message = dbcon.checkEmail(email, id);

            if (message != "")
                message = dbcon.updateEmail(email, id);

            return message;
        }
        [HttpPost]
        [Authorize]
        public string UpdatePassword(string oldPassword, string newPassword1, string newPassword2)
        {
            var message = "";
            var id = (int)Session["id"];

            if (oldPassword == "" || newPassword1 == "" || newPassword2 == "")
            {
                message = "Privalomi slaptažodžių laukai.";
                return message;
            }
            else if (newPassword1 != newPassword2)
            {
                message = "Slaptažodis ir partotinas slaptažodis nesutampa.";
                return message;
            }
            if (dbcon.checkPassword(id, oldPassword))
            {
                message = "Slaptažodis nesutampa su esamuoju slaptažodžiu.";
                return message;
            }

            message = dbcon.updatePassword(id, newPassword1);
            return message;
        }
        [HttpPost]
        public JsonResult updatePlays(int songId)
        {
            if (dbcon.updatePlays(songId))
            {
                return Json("updated plays  successful", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }




    }
}