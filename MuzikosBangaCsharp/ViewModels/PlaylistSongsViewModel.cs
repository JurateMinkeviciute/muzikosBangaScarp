using MuzikosBangaCsharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MuzikosBangaCsharp.ViewModels
{
    public class PlaylistSongsViewModel : Playlist
    {
        public PlaylistSongsViewModel()
        {
            songs = new List<SongWithArtist>();
            dropdownList = new List<DropdownViewModel>();
            songsIds = new List<int>();
        }
        public int numberOfSongs { get; set; }
        public List<SongWithArtist> songs { get; set; }
        public List<DropdownViewModel> dropdownList { get; set; }
        public List<int> songsIds { get; set; }
    }
    public class SongWithArtist : Song
    {
        public string nameOfArtist { get; set; }
    }
    public class DropdownViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}