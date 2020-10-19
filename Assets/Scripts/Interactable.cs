using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    [System.Serializable]
    public class Action{
        public Color color;
        public Sprite sprite;
        public string title;
    }

    public string title;
    public Action[] options;

    void Start(){
        if(title == "" || title == null){
            title = gameObject.name;
        }
    }

    void OnMouseOver(){
        if(Input.GetMouseButtonDown(1)){
            // Dit au Canvas de faire apparaître le menu
            RadialMenuSpawner.ins.SpawnMenu(this);
        }
    }
}
