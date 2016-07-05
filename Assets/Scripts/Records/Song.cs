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
