using System.Collections;
using System.IO;

public class Song {
    public string name;
    public string artist;
    public string path;

    public Song(string name, string artist = "", string path = "") {
        this.name = name;
        this.artist = artist;
        this.path = path;
    }
}

public class RecordInfo {
    public static string[] supportedExtensions = new string[] {
        "wav", "mp3", "mp4", "m4a", "flac", "aif", "aiff"
    };

    public string artist;
    public string name;
    public byte[] imageData;
    public Song[] songs;

    public RecordInfo(string artist, string name, byte[] imageData, Song[] songs) {
        this.artist = artist;
        this.name = name;
        this.imageData = imageData;
        this.songs = songs;
    }

    public string GetFullName() {
        return artist + " - " + name;
    }

    public static RecordInfo[] LoadDummyRecords(int amount) {
        Song[] songs = new Song[12];
        for (int i = 0; i < songs.Length; i++) {
            songs[i] = new Song("Song #" + i);
        }

        RecordInfo[] records = new RecordInfo[amount];
        for (int i = 0; i < records.Length; i++) {
            records[i] = new RecordInfo("Temp Artist", "Temp Album #" + i, new byte[0], songs);
        }
        return records;
    }

    public static RecordInfo[] LoadUserLibrary(int limit) {
        // Get file
        DirectoryInfo sourceDir = new DirectoryInfo(Util.GetConfigValue("source"));
        ArrayList listOfFileInfos = new ArrayList();
        foreach (string ext in supportedExtensions) {
            listOfFileInfos.Add(sourceDir.GetFiles("*." + ext, SearchOption.AllDirectories));
        }

        // Get tags
        Hashtable albums = new Hashtable();
        int total = 0;
        foreach (FileInfo[] fileInfos in listOfFileInfos) {
            foreach (FileInfo fileInfo in fileInfos) {
                TagLib.File file = TagLib.File.Create(fileInfo.FullName);
                string artist = file.Tag.FirstPerformer != null ? file.Tag.FirstPerformer : "Unknown";
                string album = file.Tag.Album != null ? file.Tag.Album : "Unknown";
                string song = file.Tag.Title != null ? file.Tag.Title : fileInfo.Name;
                byte[] image = file.Tag.Pictures.Length > 0 ? file.Tag.Pictures[0].Data.Data : new byte[0];
                string key = album;
                if (albums.ContainsKey(key)) {
                    object[] value = (object[])albums[key];
                    ArrayList songList = (ArrayList)(value[3]);
                    songList.Add(new Song(song, artist, fileInfo.FullName));
                } else if (total < limit && image.Length > 0) {
                    total++;
                    ArrayList songList = new ArrayList();
                    songList.Add(new Song(song, artist, fileInfo.FullName));
                    albums.Add(key, new object[] { artist, album, image, songList });
                }
            }
        }

        // Get RecordInfo
        RecordInfo[] infos = new RecordInfo[albums.Count];
        int index = 0;
        foreach (string key in albums.Keys) {
            object[] value = (object[])albums[key];
            ArrayList songArrayList = (ArrayList)value[3];
            Song[] songs = new Song[songArrayList.Count];
            for (int i = 0; i < songs.Length; i++) {
                songs[i] = (Song)songArrayList[i];
            }
            infos[index++] = new RecordInfo((string)value[0], (string)value[1], (byte[])value[2], songs);
        }
        return infos;
    }
}
