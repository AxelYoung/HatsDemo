using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.IO;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    [SerializeField] float speed;
    [SerializeField] int jumpForce;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] SpriteRenderer hatSprite;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;

    [HideInInspector] public int wearingHatID = -1;

    public override void OnStartClient() {
        base.OnStartClient();
        if (PlayerPrefs.GetInt("PlayerID", -1) == -1) {
            StartCoroutine(GetPlayerID());
        }
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (isLocalPlayer) {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
            if (Input.GetKeyDown(KeyCode.Space) && onGround()) {
                rb.AddForce(Vector2.up * jumpForce);
            }
            if (rb.velocity.x < 0) {
                transform.eulerAngles = new Vector3(0, 180, 0);
            } else if (rb.velocity.x > 0) {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        anim.SetBool("grounded", onGround());
        anim.SetFloat("vel", rb.velocity.y);
    }

    IEnumerator GetPlayerID() {
        using (UnityWebRequest web = UnityWebRequest.Get(HatNetworking.serverDir + "NewPlayerID.php")) {
            yield return web.SendWebRequest();
            PlayerPrefs.SetInt("PlayerID", int.Parse(web.downloadHandler.text));
        }
    }

    bool onGround() {
        return Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - (transform.localScale.y / 2f)), transform.localScale.x / 2f, groundLayer);
    }

    public void SetHatSprite() {
        if (wearingHatID != -1) {
            CMDHatSpriteData(this, HatNetworking.hats[wearingHatID].data);
        } else {
            CMDHatSpriteNull(this);
        }
    }

    [Command(requiresAuthority = false)]
    void CMDHatSpriteData(Player goal, byte[] data) {
        RPCHatSpriteData(goal, data);
    }

    [Command(requiresAuthority = false)]
    void CMDHatSpriteNull(Player goal) {
        RPCHatSpriteNull(goal);
    }

    [ClientRpc]
    void RPCHatSpriteData(Player goal, byte[] data) {
        Sprite sprite = HatNetworking.DataToSprite(data);
        goal.hatSprite.sprite = sprite;
    }

    [ClientRpc]
    void RPCHatSpriteNull(Player goal) {
        goal.hatSprite.sprite = null;
    }
}
