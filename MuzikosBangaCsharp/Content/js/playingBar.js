$(document).ready(function () {

    var newPlaylist = new Array();
    audioElement = new Audio();
    updateVolumeProgressBar(audioElement.audio);

    $.get("../../getRandom10Songs", function (data) {

        var list = data;
        $.each(list, function (index, item) {
            newPlaylist[index] = item;
        });
        setTrack(newPlaylist[0], newPlaylist, false);
    });



    $("#nowPlayingBarContainer").on("mousedown touchstart mousemove touchmove", function (e) {
        e.preventDefault();
    });


    $(".playbackBar .progressBar").mousedown(function () {
        mouseDown = true;
    });

    $(".playbackBar .progressBar").mousemove(function (e) {
        if (mouseDown == true) {
            //Set time of song, depending on position of mouse
            timeFromOffset(e, this);
        }
    });

    $(".playbackBar .progressBar").mouseup(function (e) {
        timeFromOffset(e, this);
    });


    $(".volumeBar .progressBar").mousedown(function () {
        mouseDown = true;
    });

    $(".volumeBar .progressBar").mousemove(function (e) {
        if (mouseDown == true) {

            var percentage = e.offsetX / $(this).width();

            if (percentage >= 0 && percentage <= 1) {
                audioElement.audio.volume = percentage;
            }
        }
    });

    $(".volumeBar .progressBar").mouseup(function (e) {
        var percentage = e.offsetX / $(this).width();

        if (percentage >= 0 && percentage <= 1) {
            audioElement.audio.volume = percentage;
        }
    });

    $(document).mouseup(function () {
        mouseDown = false;
    });




});

function timeFromOffset(mouse, progressBar) {
    var percentage = mouse.offsetX / $(progressBar).width() * 100;
    var seconds = audioElement.audio.duration * (percentage / 100);
    audioElement.setTime(seconds);
}

function prevSong() {
    if (audioElement.audio.currentTime >= 3 || currentIndex == 0) {
        audioElement.setTime(0);
    }
    else {
        currentIndex = currentIndex - 1;
        setTrack(currentPlaylist[currentIndex], currentPlaylist, true);
    }
}

function nextSong() {
    if (repeat == true) {
        audioElement.setTime(0);
        playSong();
        return;
    }

    if (currentIndex == currentPlaylist.length - 1) {
        currentIndex = 0;
    }
    else {
        currentIndex++;
    }

    var trackToPlay = shuffle ? shufflePlaylist[currentIndex] : currentPlaylist[currentIndex];
    setTrack(trackToPlay, currentPlaylist, true);
}

function setRepeat() {
    repeat = !repeat;
    var imageName = repeat ? "repeat-active.png" : "repeat.png";
    $(".controlButton.repeat img").attr("src", "~/Content/images/icons/" + imageName);
}

function setMute() {
    audioElement.audio.muted = !audioElement.audio.muted;

    var imageName = audioElement.audio.muted ? "volume-mute.png" : "volume.png";
    $(".controlButton.volume img").attr("src", "~/Content/images/icons/" + imageName);
}

function setShuffle() {
    shuffle = !shuffle;
    var imageName = shuffle ? "shuffle-active.png" : "shuffle.png";
    $(".controlButton.shuffle img").attr("src", "~/Content/images/icons/" + imageName);

    if (shuffle == true) {
        //Randomize playlist
        shuffleArray(shufflePlaylist);
        currentIndex = shufflePlaylist.indexOf(audioElement.currentlyPlaying.id);
    }
    else {
        //shuffle has been deactivated
        //go back to regular playlist
        currentIndex = currentPlaylist.indexOf(audioElement.currentlyPlaying.id);
    }

}

function shuffleArray(a) {
    var j, x, i;
    for (i = a.length; i; i--) {
        j = Math.floor(Math.random() * i);
        x = a[i - 1];
        a[i - 1] = a[j];
        a[j] = x;
    }
}


function setTrack(trackId, newPlaylist, play) {

    if (newPlaylist != currentPlaylist) {
        currentPlaylist = newPlaylist;
        shufflePlaylist = currentPlaylist.slice();
        shuffleArray(shufflePlaylist);
    }
    if (shuffle == true) {
        currentIndex = shufflePlaylist.indexOf(trackId);
    }
    else {
        currentIndex = currentPlaylist.indexOf(trackId);
    }
    pauseSong();

    $.post("getSongJson", { songId: trackId }, function (data) {

        var track = JSON.parse(data);
        $(".trackName span").text(track.title);

        $.post("getArtistJson", { artistId: track.artist }, function (data) {
            var artist = JSON.parse(data);
            $(".trackInfo .artistName span").text(artist.name);
            $(".trackInfo .artistName span").attr("onclick", "openPage('artist?id=" + artist.id + "')");
        });

        $.post("getAlbumJson", { albumId: track.album }, function (data) {
            var album = JSON.parse(data);
            $(".content .albumLink img").attr("src", album.artworkPath);
            $(".content .albumLink img").attr("onclick", "openPage('album?id=" + album.id + "')");
            $(".trackInfo .trackName span").attr("onclick", "openPage('album?id=" + album.id + "')");
        });


        audioElement.setTrack(track);

        if (play == true) {
            playSong();
        }
    });

}

function playSong() {

    if (audioElement.audio.currentTime == 0) {
        $.post("updatePlays", { songId: audioElement.currentlyPlaying.id });
    }

    $(".controlButton.play").hide();
    $(".controlButton.pause").show();
    audioElement.play();
}

function pauseSong() {
    $(".controlButton.play").show();
    $(".controlButton.pause").hide();
    audioElement.pause();
}