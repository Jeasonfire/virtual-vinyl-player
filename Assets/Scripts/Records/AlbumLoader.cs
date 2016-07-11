using System.Collections;
using System.Threading;
using System.IO;

public class AlbumLoader {
    public static string[] supportedExtensions = new string[] {
        "wav", "ogg", "mp3", "mp4", "m4a", "flac", "aif", "aiff"
    };

    private bool loading = true;
    private ArrayList loadedAlbums = new ArrayList();
    private int latestPoppedIndex = -1;
    private int maxLoadableRecords;
    private Thread loadingThread;

    public AlbumLoader (int maxLoadableRecords = -1) {
        this.maxLoadableRecords = maxLoadableRecords;
    }
    
    public void Load () {
        loadingThread = new Thread(new ThreadStart(() => {
            SearchForSongs(new DirectoryInfo(Util.GetConfigValue("source")));
            loading = false;
        }));
        loadingThread.Start();
    }

    private void SearchForSongs(DirectoryInfo sourceDir) {
        if (maxLoadableRecords != -1 && loadedAlbums.Count >= maxLoadableRecords) {
            return;
        }
        foreach (string ext in supportedExtensions) {
            FileInfo[] songFiles = sourceDir.GetFiles("*." + ext, SearchOption.TopDirectoryOnly);
            if (songFiles.Length == 0) {
                continue;
            }
            string artist = "Unknown";
            string albumName = "Unknown";
            byte[] albumArtFront = new byte[0];
            byte[] albumArtBack = new byte[0];
            Song[] songs = new Song[songFiles.Length];
            int songIndex = 0;
            foreach (FileInfo info in songFiles) {
                TagLib.Tag tag = TagLib.File.Create(info.FullName).Tag;
                /* Artist */
                if (tag.FirstPerformer != null) {
                    artist = tag.FirstPerformer;
                }
                /* Album name */
                if (tag.Album != null) {
                    albumName = tag.Album;
                } else if (albumName == "Unknown") {
                    albumName = sourceDir.Name;
                }
                /* Album pictures */
                foreach (TagLib.IPicture pic in tag.Pictures) {
                    switch (pic.Type) {
                        case TagLib.PictureType.FrontCover:
                            albumArtFront = pic.Data.Data;
                            break;
                        case TagLib.PictureType.BackCover:
                            albumArtBack = pic.Data.Data;
                            break;
                    }
                }
                /* Song name */
                string songName;
                if (tag.Title != null) {
                    songName = tag.Title;
                } else {
                    songName = info.Name.Replace("." + ext, "");
                }
                /* Song done! */
                songs[songIndex++] = new Song(songName, artist, info.FullName);
            }
            loadedAlbums.Add(new Album(artist, albumName, albumArtFront, albumArtBack, songs));
        }
        foreach (DirectoryInfo subDir in sourceDir.GetDirectories("*", SearchOption.TopDirectoryOnly)) {
            SearchForSongs(subDir);
        }
    }

    private int GetLoadedRecordsQuantity () {
        return loadedAlbums.Count;
    }

    public Album PopLatestRecordInfo () {
        if (latestPoppedIndex < GetLoadedRecordsQuantity() - 1) {
            return (Album)loadedAlbums[++latestPoppedIndex];
        } else {
            return null;
        }
    }

    public bool IsDone () {
        return !loading && latestPoppedIndex + 1 >= loadedAlbums.Count;
    }
}
