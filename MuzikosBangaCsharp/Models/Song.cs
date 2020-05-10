using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MuzikosBangaCsharp.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime signUpDate { get; set; }
        public string profilePic { get; set; }
    }
    public class Album
    {
        public int id { get; set; }
        public string title { get; set; }
        public int artist { get; set; }
        public int genre { get; set; }
        public string artworkPath { get; set; }
    }
    public class Artist
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Genre
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Playlist
    {
        public int id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public DateTime dateCreated { get; set; }
    }
    public class Playlistsong
    {
        public int id { get; set; }
        public int songId { get; set; }
        public int playlistId { get; set; }
        public int playlistOrder { get; set; }
    }
    public class Song
    {
        public int id { get; set; }
        public string title { get; set; }
        public int artist { get; set; }
        public int album { get; set; }
        public int genre { get; set; }
        public string duration { get; set; }
        public string path { get; set; }
        public int albumOrder { get; set; }
        public int plays { get; set; }
        public string nameOfArtist { get; set; }
    }
}