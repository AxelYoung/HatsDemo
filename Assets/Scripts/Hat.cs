using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public static class Hat {



    public static List<HatData> hats = new List<HatData>();
    public static int readyMadeHats;

    public static bool updating = false;

    public static void UpdateHats(MonoBehaviour instance) {
        hats.Clear();
        updating = true;
        instance.StartCoroutine(GrabHats());
    }

    public static void SendHatToServer(byte[] data, MonoBehaviour instance) {
        instance.StartCoroutine(CreatePlayerHat(data));
    }

    static IEnumerator CreatePlayerHat(byte[] data) {
        WWWForm form = new WWWForm();
        form.AddField("playerID", PlayerPrefs.GetInt("PlayerID"));
        form.AddBinaryData("hatSprite", data);

        using (UnityWebRequest web = UnityWebRequest.Post("http://54.219.56.114/Server/SetPlayerHat.php", form)) {
            yield return web.SendWebRequest();
        }
    }

    static IEnumerator GrabHats() {

        using (UnityWebRequest web = UnityWebRequest.Get("http://54.219.56.114/Server/GetReadyMadeCount.php")) {
            yield return web.SendWebRequest();
            readyMadeHats = int.Parse(web.downloadHandler.text);
        }

        string[] hatDirs;

        WWWForm form = new WWWForm();
        form.AddField("playerID", PlayerPrefs.GetInt("PlayerID"));
        using (UnityWebRequest web = UnityWebRequest.Post("http://54.219.56.114/Server/GetPlayerHats.php", form)) {
            yield return web.SendWebRequest();
            hatDirs = web.downloadHandler.text.Split('-');
        }

        for (int i = 0; i < hatDirs.Length - 1; i++) {
            using (UnityWebRequest web = UnityWebRequest.Get("http://54.219.56.114/Server/" + hatDirs[i])) {
                yield return web.SendWebRequest();
                hats.Add(new HatData(BytesToSprite(web.downloadHandler.data), hatDirs[i], web.downloadHandler.data));
            }
        }

        updating = false;
    }

    public static Sprite BytesToSprite(byte[] pngBytes) {
        Texture2D texture = new Texture2D(2, 2);
        texture.filterMode = FilterMode.Point;
        texture.LoadImage(pngBytes);

        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 16);

        return sprite;
    }

    public static void RemoveHat(string dir, MonoBehaviour instance) {
        instance.StartCoroutine(CallDeleteHat(dir));
    }

    static IEnumerator CallDeleteHat(string dir) {
        WWWForm form = new WWWForm();
        form.AddField("dir", dir);

        using (UnityWebRequest web = UnityWebRequest.Post("http://54.219.56.114/Server/DeleteHat.php", form)) {
            yield return web.SendWebRequest();
        }
    }
}

public struct HatData {
    public Sprite sprite;
    public string dir;
    public byte[] data;

    public HatData(Sprite sprite, string dir, byte[] data) {
        this.sprite = sprite;
        this.dir = dir;
        this.data = data;
    }
}