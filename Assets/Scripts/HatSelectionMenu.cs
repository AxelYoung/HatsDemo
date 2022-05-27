using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Mirror;

public class HatSelectionMenu : MonoBehaviour {

    [SerializeField] Transform hatSelectMenu;
    [SerializeField] GameObject hatSelectItem;
    [SerializeField] GameObject createNewHat;

    [SerializeField] int maxHats;

    [SerializeField] ToggleGroup toggleGroup;

    [SerializeField] Image playerHat;

    [SerializeField] GameObject hatSelect;
    [SerializeField] GameObject drawingMenu;


    [SerializeField] GameObject hatMenu;
    [SerializeField] GameObject mainGUI;

    [SerializeField] DrawingBoard drawingBoard;

    [SerializeField] GameObject deleteHatButton;

    Player player;

    void CreateMenu() {
        for (int i = 0; i < hatSelectMenu.childCount; i++) {
            Destroy(hatSelectMenu.GetChild(i).gameObject);
        }
        HatNetworking.RetrieveHats(this);
        StartCoroutine(PopulateMenu());
    }

    IEnumerator PopulateMenu() {
        yield return new WaitUntil(() => HatNetworking.retrieving == false);
        for (int i = 0; i < HatNetworking.hats.Count; i++) {
            GameObject hatObject = Instantiate(hatSelectItem, hatSelectMenu); ;
            hatObject.transform.GetChild(0).GetComponent<Image>().sprite = HatNetworking.hats[i].sprite;
            Toggle toggle = hatObject.GetComponent<Toggle>();

            if (i == player.wearingHatID) {
                toggle.isOn = true;
            }
            toggle.group = toggleGroup;
            int id = i;
            toggle.onValueChanged.AddListener(delegate { StartCoroutine(SetPlayerHat(id, toggle)); });
            if (i >= HatNetworking.premadeHats) {
                Instantiate(deleteHatButton, hatObject.transform).GetComponent<Button>().onClick.AddListener(delegate { DeleteHatItem(id); });
            }
        }
        if (HatNetworking.hats.Count < maxHats) {
            Instantiate(createNewHat, hatSelectMenu).GetComponent<Button>().onClick.AddListener(OpenDrawingBoard);
        }
    }

    void DeleteHatItem(int id) {
        HatNetworking.DeleteHat(HatNetworking.hats[id].dir, this);
        StartCoroutine(SetPlayerHat());
        CreateMenu();
    }

    IEnumerator SetPlayerHat(int id = -1, Toggle toggle = null) {
        yield return new WaitUntil(() => HatNetworking.retrieving == false);
        if (toggle != null) {
            if (toggle.isOn) {
                SetPlayerHatID(id);
            } else {
                SetPlayerHatNull();
            }
        } else {
            if (id != -1) {
                SetPlayerHatID(id);
            } else {
                SetPlayerHatNull();
            }
        }
    }

    void SetPlayerHatID(int id) {
        playerHat.sprite = HatNetworking.hats[id].sprite;
        player.wearingHatID = id;
        player.SetHatSprite();
        playerHat.color = new Color(1, 1, 1, 1);
    }

    void SetPlayerHatNull() {
        playerHat.sprite = null;
        player.wearingHatID = -1;
        player.SetHatSprite();
        playerHat.color = new Color(0, 0, 0, 0);
    }

    public void OpenHatMenu() {
        if (player == null) {
            player = NetworkClient.localPlayer.GetComponent<Player>();
        }
        hatMenu.SetActive(true);
        CreateMenu();
        StartCoroutine(SetPlayerHat(player.wearingHatID));
        mainGUI.SetActive(false);
    }

    public void CloseHatMenu() {
        hatMenu.SetActive(false);
        mainGUI.SetActive(true);
    }

    public void OpenDrawingBoard() {
        hatSelect.SetActive(false);
        drawingMenu.SetActive(true);
        drawingBoard.ClearBoard();
    }

    public void CloseDrawingBoard() {
        hatSelect.SetActive(true);
        drawingMenu.SetActive(false);
        CreateMenu();
    }

}
