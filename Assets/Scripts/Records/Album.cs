public class Album {
    public string artist;
    public string name;
    public byte[] imageData;
    public Song[] songs;

    public Album(string artist, string name, byte[] imageData, Song[] songs) {
        this.artist = artist;
        this.name = name;
        this.imageData = imageData;
        this.songs = songs;
    }
}
