using UnityEngine;
using System.Collections;

public class MainDrawerHandler : MonoBehaviour
{
    public enum DrawerDirection
    {
        Right,
        Left,
        Top,
        Buttom
    }
    
    public static bool isARightDrawerIn = false;
    public static BaseDrawer ActiveRightDrawer = null;
    public static bool isALeftDrawerIn = false;
    public static BaseDrawer ActiveLeftDrawer = null;
    public static bool isAButtomDrawerIn = false;
    public static BaseDrawer ActiveButtomDrawer = null;
    public static bool isATopDrawerIn = false;
    public static BaseDrawer ActiveTopDrawer = null;
    void Start()
    {
        isALeftDrawerIn = false;
        ActiveRightDrawer = null;
        isARightDrawerIn = false;
        ActiveLeftDrawer = null;
        isAButtomDrawerIn = false;
        ActiveButtomDrawer = null;
        isATopDrawerIn = false;
        ActiveTopDrawer = null;
    }

    public static void SetDrawerToIn(DrawerDirection whichDirection,BaseDrawer whichDrawer)
    {
        switch (whichDirection)
        {
            case DrawerDirection.Right:
                {
                    isARightDrawerIn = true;
                    ActiveRightDrawer = whichDrawer;
                    break;
                }
            case DrawerDirection.Left:
                {
                    isALeftDrawerIn = true;
                    ActiveLeftDrawer = whichDrawer;
                    break;
                }
            case DrawerDirection.Top:
                {
                    isATopDrawerIn = true;
                    ActiveTopDrawer = whichDrawer;
                    break;
                }
            case DrawerDirection.Buttom:
                {
                    isAButtomDrawerIn = true;
                    ActiveButtomDrawer = whichDrawer;
                    break;
                }

        }
    }

    public static void SetDrawerToIn(DrawerDirection whichDirection)
    {
        switch (whichDirection)
        {
            case DrawerDirection.Right:
                {
                    isARightDrawerIn = true;
                    break;
                }
            case DrawerDirection.Left:
                {
                    isALeftDrawerIn = true;
                    break;
                }
            case DrawerDirection.Top:
                {
                    isATopDrawerIn = true;
                    break;
                }
            case DrawerDirection.Buttom:
                {
                    isAButtomDrawerIn = true;
                    break;
                }

        }
    }

    public static void SetDrawerToOut(DrawerDirection whichDirection)
    {
        switch (whichDirection)
        {
            case DrawerDirection.Right:
                {
                    isARightDrawerIn = false;
                    ActiveRightDrawer = null;
                    break;
                }
            case DrawerDirection.Left:
                {
                    isALeftDrawerIn = false;
                    ActiveLeftDrawer = null;
                    break;
                }
            case DrawerDirection.Top:
                {
                    isATopDrawerIn = false;
                    ActiveTopDrawer = null;
                    break;
                }
            case DrawerDirection.Buttom:
                {
                    isAButtomDrawerIn = false;
                    ActiveButtomDrawer = null;
                    break;
                }
        }
    }
    
    public static bool GetIfADrawerIsIn(DrawerDirection whichDirection)
    {
        switch (whichDirection)
        {
            case DrawerDirection.Right:
                {
                    return isARightDrawerIn;
                }
            case DrawerDirection.Left:
                {
                    return isALeftDrawerIn;
                }
            case DrawerDirection.Top:
                {
                    return isATopDrawerIn;
                }
            case DrawerDirection.Buttom:
                {
                    return isAButtomDrawerIn;
                }
        }
        return true;
    }

    public static void CloseAllOpenDrawers()
    {
        if (ActiveRightDrawer != null)
        {
            ActiveRightDrawer.GetDrawerOut();
        }
        if (ActiveLeftDrawer != null)
        {
            ActiveLeftDrawer.GetDrawerOut();
        }
        if (ActiveTopDrawer != null)
        {
            ActiveTopDrawer.GetDrawerOut();
        }
        if (ActiveButtomDrawer != null)
        {
            ActiveButtomDrawer.GetDrawerOut();
        }

    }
}