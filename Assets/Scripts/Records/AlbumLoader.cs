using System.Collections;
using System.Threading;
using System.IO;

public class AlbumLoader {
    public static string[] supportedExtensions = new string[] {
        "wav", "mp3", "mp4", "m4a", "flac", "aif", "aiff"
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
            byte[] albumArt = new byte[0];
            Song[] songs = new Song[songFiles.Length];
            int songIndex = 0;
            foreach (FileInfo info in songFiles) {
                TagLib.Tag tag = TagLib.File.Create(info.FullName).Tag;
                if (tag.FirstPerformer != null) {
                    artist = tag.FirstPerformer;
                }
                if (tag.Album != null) {
                    albumName = tag.Album;
                }
                if (tag.Pictures.Length > 0) {
                    albumArt = tag.Pictures[0].Data.Data;
                }
                string songName = tag.Title != null ? tag.Title : "Unknown Song";
                songs[songIndex++] = new Song(songName, artist, info.FullName);
            }
            loadedAlbums.Add(new Album(artist, albumName, albumArt, songs));
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
