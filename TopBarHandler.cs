using UnityEngine;
using System.Collections;

public class TopBarHandler : MonoBehaviour {
    
    void OnSwipeTopBar(SwipeGesture gesture)
    {
        if (gesture.StartSelection == this.gameObject && gesture.Direction == FingerGestures.SwipeDirection.Down)
        {
            Debug.Log("Bringin Down Menu");
        }    
    }
}
