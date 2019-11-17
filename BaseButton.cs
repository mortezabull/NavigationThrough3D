using UnityEngine;
using System.Collections;

public class BaseButton : MonoBehaviour {
    public enum ButtonStates
    {
        Normal = 0,
        Locked = 1,
    }
    public ButtonStates ButtonState = ButtonStates.Normal;
    public Sprite[] StateSprites ;
    public void SetState(ButtonStates whichState)
    {
        GetComponent<SpriteRenderer>().sprite = StateSprites[(int)whichState];
        transform.GetComponent<Collider>().enabled = (whichState == ButtonStates.Normal);
    }
}
