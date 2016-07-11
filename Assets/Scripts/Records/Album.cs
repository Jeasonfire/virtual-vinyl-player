public class Album {
    public string artist;
    public string name;
    public byte[] coverFrontData;
    public byte[] coverBackData;
    public Song[] songs;

    public Album(string artist, string name, byte[] coverFrontData, byte[] coverBackData, Song[] songs) {
        this.artist = artist;
        this.name = name;
        this.coverFrontData = coverFrontData;
        this.coverBackData = coverBackData;
        this.songs = songs;
    }
}
