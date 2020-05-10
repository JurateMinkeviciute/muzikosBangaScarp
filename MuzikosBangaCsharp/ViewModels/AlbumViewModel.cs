using MuzikosBangaCsharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MuzikosBangaCsharp.ViewModels
{
    public class AlbumViewModel : Album
    {
        public AlbumViewModel()
        {
            songs = new List<Song>();
            songsIds = new List<int>();
            dropdownList = new List<DropdownViewModel>();
        }

        public List<Song> songs { get; set; }
        public List<int> songsIds { get; set; }
        public List<DropdownViewModel> dropdownList { get; set; }
        public string ArtistName { get; set; }
        public int NumberOfSongs { get; set; }
    }
}