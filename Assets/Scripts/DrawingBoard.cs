using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DrawingBoard : MonoBehaviour {

    [SerializeField] int spriteSize;
    [SerializeField] RawImage hatDisplay;

    Texture2D boardTexture;
    RawImage rawImage;

    void Awake() => rawImage = GetComponent<RawImage>();

    public void ClearBoard() {
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

    public void SaveHat() => HatNetworking.UploadHat(boardTexture.EncodeToPNG(), this);

}
