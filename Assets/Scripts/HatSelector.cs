using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Mirror;

public class HatSelector : MonoBehaviour {


    public Transform hatSelectMenu;
    public GameObject hatSelectItem;
    public GameObject createNewHat;

    public int maxHats;

    public ToggleGroup toggleGroup;

    public Image playerHat;

    public GameObject hatSelect;
    public GameObject hatCreatorMenu;


    public GameObject hatMenu;
    public GameObject mainGUI;

    public HatBoard hatBoard;

    public GameObject deleteHatButton;

    public Player player = null;

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

    void CreateMenu() {
        for (int i = 0; i < hatSelectMenu.childCount; i++) {
            Destroy(hatSelectMenu.GetChild(i).gameObject);
        }
        Hat.UpdateHats(this);
        StartCoroutine(PopulateMenu());
    }

    IEnumerator PopulateMenu() {

        yield return new WaitUntil(() => Hat.updating == false);

        for (int i = 0; i < Hat.hats.Count; i++) {
            GameObject hatObject = Instantiate(hatSelectItem, hatSelectMenu); ;
            hatObject.transform.GetChild(0).GetComponent<Image>().sprite = Hat.hats[i].sprite;
            Toggle toggle = hatObject.GetComponent<Toggle>();
            if (i == player.wearingHatID) {
                toggle.isOn = true;
            }
            toggle.group = toggleGroup;
            int del = i;
            toggle.onValueChanged.AddListener(delegate { StartCoroutine(SetPlayerHat(del, toggle)); });
            if (i >= Hat.readyMadeHats) {
                Instantiate(deleteHatButton, hatObject.transform).GetComponent<Button>().onClick.AddListener(delegate { DeleteHatItem(del); });
            }
        }
        if (Hat.hats.Count < maxHats) {
            Instantiate(createNewHat, hatSelectMenu).GetComponent<Button>().onClick.AddListener(OpenHatCreatorMenu);
        }
    }

    void DeleteHatItem(int id) {
        Hat.RemoveHat(Hat.hats[id].dir, this);
        StartCoroutine(SetPlayerHat());
        CreateMenu();
    }

    IEnumerator SetPlayerHat(int hat = -1, Toggle toggle = null) {

        yield return new WaitUntil(() => Hat.updating == false);

        if (toggle != null) {
            if (toggle.isOn) {
                playerHat.sprite = Hat.hats[hat].sprite;
                player.wearingHatID = hat;
                player.SetHatSprite();
                playerHat.color = new Color(1, 1, 1, 1);
            } else {
                playerHat.sprite = null;
                player.wearingHatID = -1;
                player.SetHatSprite();
                playerHat.color = new Color(0, 0, 0, 0);
            }
        } else {
            if (hat == -1) {
                playerHat.sprite = null;
                player.wearingHatID = -1;
                player.SetHatSprite();
                playerHat.color = new Color(0, 0, 0, 0);
            } else {
                playerHat.sprite = Hat.hats[hat].sprite;
                player.wearingHatID = hat;
                player.SetHatSprite();
                playerHat.color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void OpenHatCreatorMenu() {
        hatSelect.SetActive(false);
        hatCreatorMenu.SetActive(true);
        hatBoard.CreateBoard();
    }

    public void CloseHatCreatorMenu() {
        hatSelect.SetActive(true);
        hatCreatorMenu.SetActive(false);
        CreateMenu();
    }

}
