using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RadialMenu : MonoBehaviour
{
    public Text label;
    public RadialButton buttonPrefab;
    public RadialButton selected;

    // Start is called before the first frame update
    public void SpawnButtons(Interactable obj)
    {
        StartCoroutine(AnimateButtons(obj));
    }

    IEnumerator AnimateButtons(Interactable obj){
        for(int i=0; i<obj.options.Length; i++){
            RadialButton newButton = Instantiate(buttonPrefab) as RadialButton;
            newButton.transform.SetParent(transform,false);
            // Calcul la circonférence du cercle et le divise par le nombre d'option disponible
            float theta = (2*Mathf.PI/obj.options.Length)*i;
            //Calcul la position en x et y
            float xPos = Mathf.Sin(theta);
            float yPos = Mathf.Cos(theta);
            newButton.transform.localPosition = new Vector3(xPos, yPos, 0f) * 75f;
            newButton.circle.color = obj.options[i].color;
            newButton.icon.sprite = obj.options[i].sprite;
            newButton.title = obj.options[i].title;
            newButton.myMenu = this;
            newButton.Anim();
            yield return new WaitForSeconds(0.06f);
        }
    }


    void Update(){
        if(Input.GetMouseButtonUp(1)){
            if(selected){
                if(String.Equals(selected.title,"Destroy")){
                    Debug.Log(selected.title + " est selectionné");
                    
                }  
                if(String.Equals(selected.title,"Move")){
                    Debug.Log(selected.title + " est selectionné");

                }  
            }
            Destroy(gameObject);
        }
    }

}
