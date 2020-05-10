using MuzikosBangaCsharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MuzikosBangaCsharp.ViewModels
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            songs = new List<Song>();
            artists = new List<Artist>();
            albums = new List<Album>();
            dropdownList = new List<DropdownViewModel>();
        }

        public string search { get; set; }
        public List<Song> songs { get; set; }
        public List<Artist> artists { get; set; }
        public List<Album> albums { get; set; }
        public List<DropdownViewModel> dropdownList { get; set; }
    }
}