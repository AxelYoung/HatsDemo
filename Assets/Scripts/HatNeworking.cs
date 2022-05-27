using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public static class HatNetworking {

    public static List<Hat> hats = new List<Hat>();
    public static int premadeHats;

    public static bool retrieving = false;

    public static string serverDir = "http://54.219.56.114/Server/";

    public static void UploadHat(byte[] data, MonoBehaviour instance) => instance.StartCoroutine(UploadHatCoroutine(data));
    public static void RetrieveHats(MonoBehaviour instance) => instance.StartCoroutine(RetriveHatsCoroutine());
    public static void DeleteHat(string dir, MonoBehaviour instance) => instance.StartCoroutine(DeleteHatCoroutine(dir));

    static IEnumerator UploadHatCoroutine(byte[] data) {
        WWWForm form = new WWWForm();
        form.AddField("playerID", PlayerPrefs.GetInt("PlayerID"));
        form.AddBinaryData("hatSprite", data);

        using (UnityWebRequest web = UnityWebRequest.Post(serverDir + "UploadHat.php", form)) {
            yield return web.SendWebRequest();
        }
    }

    static IEnumerator RetriveHatsCoroutine() {

        hats.Clear();
        retrieving = true;

        using (UnityWebRequest web = UnityWebRequest.Get(serverDir + "PremadeHatCount.php")) {
            yield return web.SendWebRequest();
            premadeHats = int.Parse(web.downloadHandler.text);
        }

        string[] hatDirs;

        WWWForm form = new WWWForm();
        form.AddField("playerID", PlayerPrefs.GetInt("PlayerID"));
        using (UnityWebRequest web = UnityWebRequest.Post(serverDir + "RetrieveHats.php", form)) {
            yield return web.SendWebRequest();
            hatDirs = web.downloadHandler.text.Split('-');
        }

        for (int i = 0; i < hatDirs.Length - 1; i++) {
            using (UnityWebRequest web = UnityWebRequest.Get(serverDir + hatDirs[i])) {
                yield return web.SendWebRequest();
                hats.Add(new Hat(DataToSprite(web.downloadHandler.data), hatDirs[i], web.downloadHandler.data));
            }
        }

        retrieving = false;
    }

    public static Sprite DataToSprite(byte[] pngBytes) {
        Texture2D texture = new Texture2D(2, 2);
        texture.filterMode = FilterMode.Point;
        texture.LoadImage(pngBytes);

        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 16);

        return sprite;
    }

    static IEnumerator DeleteHatCoroutine(string dir) {
        WWWForm form = new WWWForm();
        form.AddField("dir", dir);

        using (UnityWebRequest web = UnityWebRequest.Post(serverDir + "DeleteHat.php", form)) {
            yield return web.SendWebRequest();
        }
    }
}