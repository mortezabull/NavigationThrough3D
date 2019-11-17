using UnityEngine;
using System.Collections;

public class BaseDrawer : MonoBehaviour {

    public MainDrawerHandler.DrawerDirection TheDrawerDirection = MainDrawerHandler.DrawerDirection.Right;
    internal Animator Comp_Animator;
    

    public virtual void BringDrawerIn()
    {
        MainDrawerHandler.SetDrawerToIn(TheDrawerDirection, this);
        if (Comp_Animator == null)
        {
            Comp_Animator = GetComponent<Animator>();
        }
        Comp_Animator.SetBool("GetIn", true);

    }

    public virtual void GetDrawerOut()
    {
        MainDrawerHandler.SetDrawerToOut(TheDrawerDirection);
        if (Comp_Animator == null)
        {
            Comp_Animator = GetComponent<Animator>();
        }
        Comp_Animator.SetBool("GetIn", false);
        
    }

}
