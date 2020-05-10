using MuzikosBangaCsharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MuzikosBangaCsharp.ViewModels
{
    public class ArtistViewModel : Song
    {
        public ArtistViewModel()
        {
            songs = new List<Song>();
            albums = new List<Album>();
            songsIds = new List<int>();
        }
        public string ArtistName { get; set; }
        public List<Song> songs { get; set; }
        public List<Album> albums { get; set; }
        public List<int> songsIds { get; set; }
    }
}