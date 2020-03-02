using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int state = 0;
    public int[] position;
    public int currentRl = 0;
    
    SpriteRenderer sprRend;
        
    public void CreateCell(int[] pos, Sprite newSpr)
    {
        state = 0;
        position = pos;
        this.transform.position = new Vector2(position[0], position[1]);
        this.gameObject.AddComponent<BoxCollider>();
        this.gameObject.AddComponent<SpriteRenderer>();
        sprRend = this.gameObject.GetComponent<SpriteRenderer>();
        SetSprite(newSpr);
    }

    public void SetState(int newStatus, int r)
    {
        state = newStatus;
        currentRl = r;
        ChangeColour();
    }
    
    void SetSprite(Sprite newSpr)
    {
        sprRend.sprite = newSpr;
        sprRend.color = new Color(state, state, state);
    }

    public void SwitchState()
    {
        state = state == 0 ? 1 : 0;
        if (state == 0)
            sprRend.color = Color.white;
        else if (state == 1)
            sprRend.color = Color.black;
        //sprRend.color = new Color(state, state, state);
    }

    public void ChangeColour()
    {
        if (state == 0)
            sprRend.color = Color.white;
        else if (state == 1)
            sprRend.color = Color.black;
        //sprRend.color = new Color(state, state, state);
    }
}
