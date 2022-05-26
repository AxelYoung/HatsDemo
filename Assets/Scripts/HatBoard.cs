using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HatBoard : MonoBehaviour {

    Texture2D boardTexture;
    RawImage rawImage;

    public int spriteSize;

    public RawImage hatDisplay;

    void Awake() {
        rawImage = GetComponent<RawImage>();
        CreateBoard();
    }

    public void CreateBoard() {
        boardTexture = new Texture2D(spriteSize, spriteSize);
        for (int x = 0; x < spriteSize; x++) {
            for (int y = 0; y < spriteSize; y++) {
                boardTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }
        boardTexture.filterMode = FilterMode.Point;
        boardTexture.wrapMode = TextureWrapMode.Clamp;
        boardTexture.Apply();
        rawImage.texture = boardTexture;
        hatDisplay.texture = boardTexture;
    }

    public void SetPixel(int x, int y, Color color) {
        boardTexture.SetPixel(x, y, color);
        boardTexture.Apply();
    }

    public void SaveHat() {
        byte[] bytes = boardTexture.EncodeToPNG();
        Hat.SendHatToServer(bytes, this);
    }
}
