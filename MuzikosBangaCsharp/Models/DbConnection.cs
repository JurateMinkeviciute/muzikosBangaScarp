    using MuzikosBangaCsharp.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MuzikosBangaCsharp.Models
{
    public class DbConnection : IDisposable
    {
        private MySqlConnection con;
        private string connectionString;
        MySqlDataReader reader;

        public DbConnection()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["MuzikaConnectionString"].ConnectionString;
        }

        public bool OpenConnection()
        {
            con = new MySqlConnection(this.connectionString);
            //try
            {
                //if (con == null)
                //{                           // we make sure we're only opening connection once.
                //    con.Open();
                //        return true;
                //    }
                //}
                con.Open();
                return true;
                //catch (MySqlException ex)
                //{
                //    switch (ex.Number)
                //    {
                //        case 0:
                //            Debug.WriteLine("Cannot connect to server.  Contact administrator");
                //            break;

                //        case 1045:
                //            Debug.WriteLine("Invalid username/password, please try again");
                //            break;
                //    }
                //    return false;
                //}
            }
        }
        public bool CloseConnection()
        {
            try
            {
                if (con != null)
                { // I'm making stuff up here
                    con.Close();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return false;
        }
        // Implementing IDisposable method
        public void Dispose()
        {
            CloseConnection();
        }

        //public DataTable getFromDatabase(string SQL)
        //{
        //    OpenConnection();

        //    DataTable rt = new DataTable();
        //    DataSet ds = new DataSet();
        //    OdbcCommand cmd = new OdbcCommand(SQL, con);
        //    da.SelectCommand = cmd;
        //    da.Fill(ds);
        //    try
        //    {
        //        rt = ds.Tables[0];
        //    }
        //    catch
        //    {
        //        rt = null;
        //    }
        //    return rt;
        //}

        public bool insertIntoDatabase(string SQL)
        {
            OpenConnection();

            MySqlCommand cmd = new MySqlCommand(SQL, con);
            con.Open();
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Person> getUsersFromDb(string query)
        {
            var ListOfUsers = new List<Person>();

            if (this.OpenConnection() == true)
            {

                MySqlCommand cmd = new MySqlCommand(query, con);

                using (reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        var person = new Person();
                        person.id = reader.GetString(0);
                        person.firstname = reader.GetString(1);
                        person.lastname = reader.GetString(2);
                        person.email = reader.GetString(3);
                        ListOfUsers.Add(person);
                    }
                }
            }
            this.CloseConnection();
            return ListOfUsers;
        }
        public List<Album> getAlbumsFromDb(string query)
        {
            List<Album> ListOfAlbum = new List<Album>();

            if(this.OpenConnection()) { 

                MySqlCommand cmd = new MySqlCommand(query, con);

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var album = new Album();
                        album.id = reader.GetInt32(0);
                        album.title = reader.GetString(1);
                        album.artist = reader.GetInt32(2);
                        album.genre = reader.GetInt32(3);
                        album.artworkPath = reader.GetString(4);
                        ListOfAlbum.Add(album);
                    }
                }
            }
            this.CloseConnection();
            return ListOfAlbum;
        }
        public User checkLogin(LoginViewModel model, string query) {

            User user = new User();
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT id, username FROM users Where username = @username and password = @password", con);

                cmd.Parameters.AddWithValue("@username", model.Username);
                cmd.Parameters.AddWithValue("@password", model.Password);

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.id = reader.GetInt32("id");
                        user.username = reader.GetString("username");
                    }
                }
            }
            this.CloseConnection();
            return user;
        }
        public bool userRegistration(RegisterViewModel model, string query)
        {
            bool success = false;
            OpenConnection();
            {

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@username", model.Username);
                cmd.Parameters.AddWithValue("@firstName", model.FirstName);
                cmd.Parameters.AddWithValue("@lastName", model.LastName);
                cmd.Parameters.AddWithValue("@email", model.Email);
                cmd.Parameters.AddWithValue("@password", model.Password);
                cmd.Parameters.AddWithValue("@signUpDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@profilePic", "head-emerald.png");

                int rowAffected = cmd.ExecuteNonQuery();
                reader = cmd.ExecuteReader();

                if (rowAffected > 0)
                {
                    success = true;
                }
            }
            CloseConnection();
            return success;
        }
        public string checkEmail(string email, int id)
        {
            string message = "";
            OpenConnection();

                MySqlCommand cmdEmailInUse = new MySqlCommand("SELECT email FROM users WHERE email=@email AND id = @id ", con);

                cmdEmailInUse.Parameters.AddWithValue("@id", id);
                cmdEmailInUse.Parameters.AddWithValue("@email", email);

                var effectedRow = cmdEmailInUse.ExecuteNonQuery();
                if (effectedRow > 0)
                {
                    message = "Email is already in use";
                }
            CloseConnection();
            return message;
         }
        public List<Playlist> getYourMusic(string username)
        {
            List<Playlist> playlist = new List<Playlist>();

            OpenConnection();

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM playlists Where owner = @username LIMIT 10 ", con);

            cmd.Parameters.AddWithValue("@username", username);

            using (reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Playlist play = new Playlist()
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        owner = reader.GetString("owner"),
                        dateCreated = reader.GetDateTime("dateCreated")
                    };
                    playlist.Add(play);
                }
            }
            CloseConnection();
            return playlist;
        }
        public SearchViewModel getSearchSongs(string search, SearchViewModel searchModel)
        {
            OpenConnection();

            MySqlCommand cmdSongs = new MySqlCommand("SELECT * FROM songs Where title LIKE @search LIMIT 10 ", con);

            cmdSongs.Parameters.AddWithValue("@search", "%" + search + "%");

            using (reader = cmdSongs.ExecuteReader())
            {
                while (reader.Read())
                {
                    Song song = new Song()
                    {
                        id = reader.GetInt32("id"),
                        title = reader.GetString("title"),
                        artist = reader.GetInt32("artist"),
                        album = reader.GetInt32("album"),
                        genre = reader.GetInt32("genre"),
                        duration = reader.GetString("duration"),
                        path = reader.GetString("path"),
                        albumOrder = reader.GetInt32("albumOrder"),
                        plays = reader.GetInt32("plays"),
                    };
                    searchModel.songs.Add(song);
                }
            }
            foreach (var song in searchModel.songs)
            {

                MySqlCommand cmdArtistName = new MySqlCommand("SELECT name FROM artists Where id = @songArtistId LIMIT 10 ", con);

                cmdArtistName.Parameters.AddWithValue("@songArtistId", song.artist);

                using (reader = cmdArtistName.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        song.nameOfArtist = reader.GetString("name");
                    }
                }
            }
            return searchModel;
        }
        public SearchViewModel getSearchArtists(string search, SearchViewModel searchModel)
        {
            OpenConnection();
            MySqlCommand cmdArtists = new MySqlCommand("SELECT * FROM artists Where name LIKE @search LIMIT 10 ", con);

            cmdArtists.Parameters.AddWithValue("@search", "%" + search + "%");

            using (reader = cmdArtists.ExecuteReader())
            {
                while (reader.Read())
                {
                    Artist artist = new Artist()
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name")
                    };
                    searchModel.artists.Add(artist);
                }
            }
            CloseConnection();
            return searchModel;
        }
        public SearchViewModel getSearchAlbums(string search, SearchViewModel searchModel)
        {
            OpenConnection();
            MySqlCommand cmdAlbums = new MySqlCommand("SELECT * FROM albums Where title LIKE @search LIMIT 10 ", con);

            cmdAlbums.Parameters.AddWithValue("@search", "%" + search + "%");

            using (reader = cmdAlbums.ExecuteReader())
            {
                while (reader.Read())
                {
                    Album album = new Album()
                    {
                        id = reader.GetInt32("id"),
                        title = reader.GetString("title"),
                        artworkPath = reader.GetString("artworkPath")
                    };
                    searchModel.albums.Add(album);
                }
            }
            CloseConnection();
            return searchModel;
        }
        public AlbumViewModel getAlbum(int id)
        {
            AlbumViewModel albumModel = new AlbumViewModel();

            OpenConnection();

            MySqlCommand cmdAlbum = new MySqlCommand("SELECT * FROM albums Where id = @id ", con);

            cmdAlbum.Parameters.AddWithValue("@id", id);

            using (reader = cmdAlbum.ExecuteReader())
            {
                while (reader.Read())
                {
                    albumModel.id = reader.GetInt32("id");
                    albumModel.title = reader.GetString("title");
                    albumModel.artist = reader.GetInt32("artist");
                    albumModel.artworkPath = reader.GetString("artworkPath");
                }
            }
            MySqlCommand cmdArtist = new MySqlCommand("SELECT * FROM artists Where id = @artistId ", con);

            cmdArtist.Parameters.AddWithValue("@artistId", albumModel.artist);

            using (reader = cmdArtist.ExecuteReader())
            {
                while (reader.Read())
                {
                    albumModel.ArtistName = reader.GetString("name");
                }
            }
            MySqlCommand cmdAlbumSongs = new MySqlCommand("SELECT * FROM songs Where album = @albumId ", con);

            cmdAlbumSongs.Parameters.AddWithValue("@albumId", id);
            using (reader = cmdAlbumSongs.ExecuteReader())
            {
                while (reader.Read())
                {
                    Song song = new Song()
                    {
                        id = reader.GetInt32("id"),
                        title = reader.GetString("title"),
                        duration = reader.GetString("duration")
                    };
                    albumModel.songs.Add(song);
                }
             }
            CloseConnection();
            return albumModel;
        }
        public ArtistViewModel getArtist(int id)
        {
            OpenConnection();
            ArtistViewModel artistwithSongs = new ArtistViewModel();

            MySqlCommand cmdArtist = new MySqlCommand("SELECT * FROM artists Where id = @artistId ", con);

            cmdArtist.Parameters.AddWithValue("@artistId", id);

            using (reader = cmdArtist.ExecuteReader())
            {
                while (reader.Read())
                {
                    artistwithSongs.nameOfArtist = reader.GetString("name");
                }
            }
            MySqlCommand cmdSongs = new MySqlCommand("SELECT * FROM songs WHERE artist = @artistId LIMIT 10", con);

            cmdSongs.Parameters.AddWithValue("@artistId", id);

            using (reader = cmdSongs.ExecuteReader())
            {
                while (reader.Read())
                {
                    Song song = new Song()
                    {
                        id = reader.GetInt32("id"),
                        title = reader.GetString("title"),
                        artist = reader.GetInt32("artist"),
                        duration = reader.GetString("duration"),
                        path = reader.GetString("path")
                    };
                    artistwithSongs.songs.Add(song);
                }
            }

            MySqlCommand cmdAlbum = new MySqlCommand("SELECT * FROM albums WHERE artist = @artistId ", con);

            cmdAlbum.Parameters.AddWithValue("@artistId", id);

            using (reader = cmdAlbum.ExecuteReader())
            {
                while (reader.Read())
                {
                    Album album = new Album()
                    {
                        id = reader.GetInt32("id"),
                        title = reader.GetString("title"),
                        artist = reader.GetInt32("artist"),
                        artworkPath = reader.GetString("artworkPath")
                    };
                    artistwithSongs.albums.Add(album);
                }
            }
            CloseConnection();
            return artistwithSongs;
        }
        public PlaylistSongsViewModel getPlaylistSongs(int id, string username)
        {
            PlaylistSongsViewModel playlistSongs = new PlaylistSongsViewModel();
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM playlists Where id = @playlistId", con);

                cmd.Parameters.AddWithValue("@playlistId", id);

                using (reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        playlistSongs.id = reader.GetInt32("id");
                        playlistSongs.name = reader.GetString("name");
                        playlistSongs.owner = reader.GetString("owner");
                    }
                }

                MySqlCommand cmdSongs = new MySqlCommand("SELECT * From playlistsongs INNER JOIN songs ON playlistsongs.songId = songs.id WHERE playlistsongs.playlistId = @playlistId", con);

                cmdSongs.Parameters.AddWithValue("@playlistId", id);

                using (reader = cmdSongs.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SongWithArtist song = new SongWithArtist()
                        {
                            id = reader.GetInt32("songId"),
                            title = reader.GetString("title"),
                            artist = reader.GetInt32("artist"),
                            duration = reader.GetString("duration")
                        };
                        playlistSongs.songs.Add(song);
                    }
                }
                foreach (var song in playlistSongs.songs)
                {

                    MySqlCommand cmdArtistName = new MySqlCommand("SELECT name FROM artists Where id = @artistId ", con);

                    cmdArtistName.Parameters.AddWithValue("@artistId", song.artist);

                    using (reader = cmdArtistName.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            song.nameOfArtist = reader.GetString("name");
                        }
                    }
                }
                MySqlCommand cmdGetPlaylists = new MySqlCommand("SELECT id, name FROM playlists WHERE owner= @username ", con);

                cmdGetPlaylists.Parameters.AddWithValue("@username", username);

                using (reader = cmdGetPlaylists.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DropdownViewModel dropdown = new DropdownViewModel()
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name")
                        };
                        playlistSongs.dropdownList.Add(dropdown);
                    }
                }
            }
            this.CloseConnection();
            return playlistSongs;
        }
        public List<DropdownViewModel> getPlaylistsDropdown(string username)
        {
            List<DropdownViewModel> dropdownViewModels = new List<DropdownViewModel>();
            OpenConnection();

            MySqlCommand cmdGetPlaylists = new MySqlCommand("SELECT id, name FROM playlists WHERE owner= @username ", con);

            cmdGetPlaylists.Parameters.AddWithValue("@username", username);

            using (reader = cmdGetPlaylists.ExecuteReader())
            {
                while (reader.Read())
                {
                    DropdownViewModel dropdown = new DropdownViewModel() {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name")
                    };
                    dropdownViewModels.Add(dropdown);
                }
            }
            CloseConnection();
            return dropdownViewModels;
        }

        // For Random Music's Track
        public List<int> getRandom10Songs()
        {
            List<int> ids10RandomSongs = new List<int>();
            OpenConnection();

            MySqlCommand cmdGetPlaylists = new MySqlCommand("SELECT id FROM songs ORDER BY RAND() LIMIT 10", con);

            using (reader = cmdGetPlaylists.ExecuteReader())
            {
                int i = 0;
                while (reader.Read())
                {
                    //ids10RandomSongs[i] = reader.GetInt32("id");
                    ids10RandomSongs.Add( reader.GetInt32("id") );
                }
            }
            CloseConnection();
            return ids10RandomSongs;
        }

        // AJAX Calls
        public bool addToPlaylist(int playlistId, int songId)
        {
            Playlist playlist = new Playlist();
            int order = 0;
            OpenConnection();

            MySqlCommand cmdGetOrder = new MySqlCommand("SELECT MAX(playlistOrder) + 1 as playlistOrder FROM playlistSongs WHERE playlistId=@playlistId" , con);

            cmdGetOrder.Parameters.AddWithValue("@playlistId", playlistId);

            using (reader = cmdGetOrder.ExecuteReader())
            {
                while (reader.RecordsAffected > 0)
                {
                    order = reader.GetInt32("playlistOrder");
                }
            }

            MySqlCommand cmdCreatePlaylistSongs = new MySqlCommand("INSERT INTO playlistsongs (songId, playlistId, playlistOrder) VALUES (@songId, @playlistId, @playlistOrder) ", con);

            cmdCreatePlaylistSongs.Parameters.AddWithValue("@songId", songId);
            cmdCreatePlaylistSongs.Parameters.AddWithValue("@playlistId", playlistId);
            cmdCreatePlaylistSongs.Parameters.AddWithValue("@playlistOrder", order);

            var effectedRows = cmdCreatePlaylistSongs.ExecuteNonQuery();
            {
                if (effectedRows > 0)
                {
                    CloseConnection();
                    return true;
                }
            }

            CloseConnection();
            return false;
        }
        public bool createPlaylist(string name, string username, DateTime dateTime)
        {
            Playlist playlist = new Playlist();
            OpenConnection();

            MySqlCommand cmdCreatePlaylists = new MySqlCommand("INSERT INTO playlists (name, owner, dateCreated) VALUES (@name, @owner, @dateCreated) ", con);

            cmdCreatePlaylists.Parameters.AddWithValue("@name", name);
            cmdCreatePlaylists.Parameters.AddWithValue("@owner", username);
            cmdCreatePlaylists.Parameters.AddWithValue("@dateCreated", dateTime);

            var effectedRows = cmdCreatePlaylists.ExecuteNonQuery();
            {
                if (effectedRows > 0)
                {
                    CloseConnection();
                    return true;
                }
            }
            CloseConnection();
            return false;
        }
        public bool deletePlaylist(int playlistId)
        {
            OpenConnection();

            MySqlCommand cmdDeletePlaylist = new MySqlCommand("DELETE FROM playlists WHERE id = @id", con);

            cmdDeletePlaylist.Parameters.AddWithValue("@id", playlistId);

            var effectedRows = cmdDeletePlaylist.ExecuteNonQuery();
            {
                if (effectedRows > 0)
                {
                    CloseConnection();
                    return true;
                }
            }
            CloseConnection();
            return false;
        }
        public Album getAlbumJson(int albumId)
        {
            Album album = new Album();
            OpenConnection();

            MySqlCommand cmdAlbum = new MySqlCommand("SELECT * FROM albums Where id = @id ", con);

            cmdAlbum.Parameters.AddWithValue("@id", albumId);

            using (reader = cmdAlbum.ExecuteReader())
            {
                while (reader.Read())
                {
                    album.id = reader.GetInt32("id");
                    album.title = reader.GetString("title");
                    album.artist = reader.GetInt32("artist");
                    album.artworkPath = reader.GetString("artworkPath");
                }
            }
            CloseConnection();
            return album;
        }
        public Artist getArtistJson(int artistId)
        {
            Artist artist = new Artist();
            OpenConnection();

            MySqlCommand cmdGetPlaylists = new MySqlCommand("SELECT * FROM artists WHERE id = @artistId ", con);

            cmdGetPlaylists.Parameters.AddWithValue("@artistId", artistId);

            using (reader = cmdGetPlaylists.ExecuteReader())
            {
                while (reader.Read())
                {
                    artist.id = reader.GetInt32("id");
                    artist.name = reader.GetString("name");
                }
            }
            CloseConnection();
            return artist;
        }
        public Song getSongJson(int songId)
        {
            Song song = new Song();
            OpenConnection();

            MySqlCommand cmdGetPlaylists = new MySqlCommand("SELECT * FROM songs WHERE id = @songId", con);

            cmdGetPlaylists.Parameters.AddWithValue("@songId", songId);

            using (reader = cmdGetPlaylists.ExecuteReader())
            {
                while (reader.Read())
                {
                    song.id = reader.GetInt32("id");
                    song.title = reader.GetString("title");
                    song.artist = reader.GetInt32("artist");
                    song.album = reader.GetInt32("album");
                    song.genre = reader.GetInt32("genre");
                    song.duration = reader.GetString("duration");
                    song.path = reader.GetString("path");
                    song.albumOrder = reader.GetInt32("albumOrder");
                    song.plays = reader.GetInt32("plays");
                }
            }
            CloseConnection();
            return song;
        }
        public bool removeFromPlaylist(int playlistId, int songId)
        {
            OpenConnection();

            MySqlCommand cmdDeletePlaylist = new MySqlCommand("DELETE FROM playlistSongs WHERE playlistId=@playlistId AND songId=@songId ", con);

            cmdDeletePlaylist.Parameters.AddWithValue("@playlistId", playlistId);
            cmdDeletePlaylist.Parameters.AddWithValue("@songId", songId);

            var effectedRows = cmdDeletePlaylist.ExecuteNonQuery();
            {
                if (effectedRows > 0)
                {
                    CloseConnection();
                    return true;
                }
            }
            CloseConnection();
            return false;
        }
        public string updateEmail(string email, int id)
        {
            string message = "";
            OpenConnection();
            MySqlCommand cmdUpdateEmail = new MySqlCommand("UPDATE users SET email = @email WHERE id = @id ", con);

            cmdUpdateEmail.Parameters.AddWithValue("@id", id);
            cmdUpdateEmail.Parameters.AddWithValue("@email", email);

            var effectedRows = cmdUpdateEmail.ExecuteNonQuery();
            if (effectedRows > 0)
            {
                message = "Updated successful";
            }
            CloseConnection();
            return message;
        }
        public UpdateEmailViewModel getUserEmail(int id)
        {
            UpdateEmailViewModel emailModel = new UpdateEmailViewModel();

            OpenConnection();

            MySqlCommand cmdAlbums = new MySqlCommand("SELECT email FROM users Where id = @id ", con);

            cmdAlbums.Parameters.AddWithValue("@id", id);

            using (reader = cmdAlbums.ExecuteReader())
            {
                while (reader.Read())
                {
                    emailModel.email = reader.GetString("email");
                }
            }
            CloseConnection();
            return emailModel;
        }
        public bool checkPassword(int id, string oldPassword)
        {
            OpenConnection();

            MySqlCommand cmdUpdatePassword = new MySqlCommand("SELECT password FROM users WHERE id = @id ", con);

            cmdUpdatePassword.Parameters.AddWithValue("@id", id);

            using (reader = cmdUpdatePassword.ExecuteReader())
            {
                while (reader.HasRows)
                {
                    return true;
                }
            }
            CloseConnection();
            return false;
        }
        public string updatePassword(int id, string newPassword)
        {
            var message = "";

            OpenConnection();

            MySqlCommand cmdUpdatePassword = new MySqlCommand("UPDATE users SET password = @password WHERE id = @id ", con);

            cmdUpdatePassword.Parameters.AddWithValue("@id", id);
            cmdUpdatePassword.Parameters.AddWithValue("@password", newPassword);

            var effectedRows = cmdUpdatePassword.ExecuteNonQuery();
            if (effectedRows > 0)
            {
                message = "Password updated successful";
            }
            CloseConnection();
            return message;
        }
        public bool updatePlays(int id)
        {
            Playlist playlist = new Playlist();
            OpenConnection();

            MySqlCommand cmdUpdatePlays = new MySqlCommand("UPDATE songs SET plays = plays + 1 WHERE id=@songId", con);

            cmdUpdatePlays.Parameters.AddWithValue("@songId", id);

            var effectedRows = cmdUpdatePlays.ExecuteNonQuery();
            {
                if (effectedRows > 0)
                {
                    CloseConnection();
                    return true;
                }
            }
            CloseConnection();
            return false;
        }


    }
}