using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DebugUI : MonoBehaviour
{

    public GameObject RightUI;
    public GameObject LeftUI;
    public GameObject JumpUI;
    [SerializeField] Text _uiTextPlayerDmg;

    int dmgTotal;

    void Start() {
        Player.Instance.Damagable.OnDamage.Subscribe(dmg => {
            dmgTotal += dmg;
            _uiTextPlayerDmg.text = dmgTotal.ToString();
        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("a")) {
            LeftUI.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
            
        }else{
            LeftUI.GetComponent<Image>().color = new Color(1f,1f,1f);
        }

        if(Input.GetKey("d")) {
            RightUI.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
            
        }else{
            RightUI.GetComponent<Image>().color = new Color(1f,1f,1f);
        }

        if(Input.GetKey("space")) {
            JumpUI.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
            
        }else{
            JumpUI.GetComponent<Image>().color = new Color(1f,1f,1f);
        }
    }
}
