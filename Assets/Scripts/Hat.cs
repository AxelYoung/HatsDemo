using UnityEngine;

public struct Hat {
    public Sprite sprite;
    public string dir;
    public byte[] data;

    public Hat(Sprite sprite, string dir, byte[] data) {
        this.sprite = sprite;
        this.dir = dir;
        this.data = data;
    }
}