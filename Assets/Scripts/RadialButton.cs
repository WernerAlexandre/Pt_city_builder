using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image circle;
    public Image icon;
    public string title;
    public RadialMenu myMenu;
    public float speed = 8f;

    Color defaultColor;
    Color selectionColor = new Color(0f, 0.6f, 1f, 1f);

    public void Anim(){
        StartCoroutine(AnimateButtonIn());
    }

    IEnumerator AnimateButtonIn(){
        transform.localScale = Vector3.zero;
        float timer = 0f;
        while(timer < (1/speed)){
            timer += Time.deltaTime;
            transform.localScale = Vector3.one * timer * speed;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData eventData){
        myMenu.selected = this;
        defaultColor = circle.color;
        circle.color = selectionColor;
    }

    public void OnPointerExit(PointerEventData eventData){
        myMenu.selected = null;
        circle.color = defaultColor;
    }

}
