using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class RightDrawerHandler : MonoBehaviour
{
    [Serializable]
    public struct AffordanceMenuCouple
    {
        public Affordances.Names name;
        public GameObject Menu_Normal;
        public GameObject Menu_Camptionless;
    }
    public AffordanceMenuCouple[] allAfordanceMenuCouples;
    public GameObject LayoutStartingPoint;
    // !!!
    public GameObject GUIGridPointPrefab;
    public Vector2 GridSize;
    public Vector3 GridStartPoint;
    public float GridVerticalGap;
    public float GridHorizontalGap;
    private Dictionary<Vector2, GUIGridPoint> GUIGridPoints;
    public delegate void EmptyGridPoint();
    public EmptyGridPoint EmptyAllGridPoints;
    // !!!
    public GameObject CaptionPref;
    public GameObject OneDimensionalPref;
    public GameObject OneDimensionalCaptionLessPref;
    public GameObject OneDimensionalWindowPref;
    public GameObject BinaryPref;

    public Dictionary<String, GameObject> DeviceControllerDictionary;
    public GameObject TVControllerPrefab;
    public GameObject SlideControllerPrefab;
    public GameObject ChillerControllerPrefab;
    public GameObject FanControllerPrefab;

    public int NumberOfButtonsInRow = 2;
    public float VerticalGap = 0.4f;
    public float HorizontalGap = 0.8f;

    private Vector3 LayoutStartingPoint_InitialPosition;
    private int CurrentRow = 0;
    private int NumberOfBinarysInRow = 0;
    private int CurrentColoumn = 0;
    private ArrayList OldControllSet = new ArrayList();
    private FunctionalityMenuHandler Sub_FunctionalityMenuHandler;
    private List<Vector2> OneDimAffordances;
    private Animator Comp_Animator;
    private Dictionary<Affordances.Names, GameObject> DictionaryOfNormalAffordanceMenus;
    private Dictionary<Affordances.Names, GameObject> DictionaryOfCaptionlessAffordanceMenus;

    void Start()
    {
        // !!!
        CreateGrid();
        // !!!
        Comp_Animator = GetComponent<Animator>();
        LayoutStartingPoint_InitialPosition = LayoutStartingPoint.transform.localPosition;
        Sub_FunctionalityMenuHandler = GameObject.FindObjectOfType<FunctionalityMenuHandler>();
        DictionaryOfNormalAffordanceMenus = new Dictionary<Affordances.Names, GameObject>();
        DictionaryOfCaptionlessAffordanceMenus = new Dictionary<Affordances.Names, GameObject>();
        DeviceControllerDictionary = new Dictionary<string, GameObject>();

        for (int i = 0; i < allAfordanceMenuCouples.Length; i++)
        {
            DictionaryOfNormalAffordanceMenus.Add(allAfordanceMenuCouples[i].name, allAfordanceMenuCouples[i].Menu_Normal);
            DictionaryOfCaptionlessAffordanceMenus.Add(allAfordanceMenuCouples[i].name, allAfordanceMenuCouples[i].Menu_Camptionless);
        }
    }

    // !!!
    #region New Approach (Grid Based)
    void CreateGrid()
    {
        GUIGridPoints = new Dictionary<Vector2, GUIGridPoint>();
        GameObject tempGameObject;
        GUIGridPoint tempGUIGridPoint;
        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                tempGameObject = (GameObject)Instantiate(GUIGridPointPrefab);
                tempGameObject.transform.parent = this.transform;
                tempGameObject.transform.localPosition = GridStartPoint + (i * GridHorizontalGap * Vector3.right) + (j * GridVerticalGap * Vector3.down);
                tempGUIGridPoint = tempGameObject.GetComponent<GUIGridPoint>();
                tempGUIGridPoint.X = i;
                tempGUIGridPoint.Y = j;
                tempGUIGridPoint.isOccupied = false;
                GUIGridPoints.Add(new Vector2(i, j), tempGUIGridPoint);
                EmptyAllGridPoints += tempGUIGridPoint.SetToEmpty;
            }
        }

    }

    void CreateControllerSets()
    {
        List<Affordances> AssignedToController = new List<Affordances>();
        #region Create Caption
  //      CreateCaptionAt(CaptionPref.GetComponent<BaseGUIHandler>().SpaceRequired, CaptionPref);
        #endregion
        if (FunctionalityMenuHandler.SelectedDevice.ModelURL.Contains("TV"))
        {
            // import the controller
            GameObject Controller = ImportControlSet(TVControllerPrefab.GetComponent<DeviceController>().SpaceRequired, TVControllerPrefab);

            AssignHandlers(Controller, "TV", AssignedToController);

        }
        else if (FunctionalityMenuHandler.SelectedDevice.ModelURL.Contains("AirConditionerController"))
        {
            // import the controller
            GameObject Controller = ImportControlSet(ChillerControllerPrefab.GetComponent<DeviceController>().SpaceRequired, ChillerControllerPrefab);

            AssignHandlers(Controller, "Chiller", AssignedToController);

        }
        else if (FunctionalityMenuHandler.SelectedDevice.ModelURL.Contains("Air_Conditioner_Large_"))
        {
            // import the controller
            GameObject Controller = ImportControlSet(FanControllerPrefab.GetComponent<DeviceController>().SpaceRequired, FanControllerPrefab);

            AssignHandlers(Controller, "Fan", AssignedToController);

        }
        #region Create affordances based for support
        else if (FunctionalityMenuHandler.SelectedDevice.supportedMediaFormats.Count != 0)
            {
                for (int i = 0; i < FunctionalityMenuHandler.SelectedDevice.supportedMediaFormats.Count; i++)
                {
                    if (FunctionalityMenuHandler.SelectedDevice.supportedMediaFormats[i] == Affordances.MediaTypes.ppt)
                    {
                        GameObject Controller = ImportControlSet(SlideControllerPrefab.GetComponent<DeviceController>().SpaceRequired, SlideControllerPrefab);
                        AssignHandlers(Controller, "Slide", AssignedToController);
                    }
                }
            }
        #endregion
        // If there exists a pre modeled controller for the device
        else
        {
            for (int i = 0; i < FunctionalityMenuHandler.SelectedDevice.ListOfAffordances.Count; i++)
            {
                if (!AssignedToController.Contains(FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i]))
                {
                    if (FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i].Type == Affordances.Types.Binary)
                    {
                        CreateBinaryControlSetAt(BinaryPref.GetComponent<BaseGUIHandler>().SpaceRequired, BinaryPref, i);
                    }
                    else if (FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i].Type == Affordances.Types.OneDimensional)
                    {
                        CreateOneDimControlSetAt(DictionaryOfNormalAffordanceMenus[FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i].name].GetComponent<BaseGUIHandler>().SpaceRequired, DictionaryOfNormalAffordanceMenus[FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i].name], i);
                    }
                }
                

            }
            
        }

        
    }

    void CreateCaptionAt(Vector2 spaceRequired, GameObject whichController)
    {
        Vector2 startPoint = FindFirstEmpty(spaceRequired);
        GameObject tempGameObject = (GameObject)Instantiate(whichController);
        tempGameObject.GetComponent<CaptionHandler>().SetName(FunctionalityMenuHandler.SelectedDevice.name);
        tempGameObject.GetComponent<CaptionHandler>().SetDeviceIcon(FunctionalityMenuHandler.SelectedDevice.name);
        tempGameObject.transform.parent = this.transform;
        tempGameObject.transform.localPosition = (GUIGridPoints[startPoint + spaceRequired - Vector2.one].transform.localPosition + GUIGridPoints[startPoint].transform.localPosition) * .5f;
        OldControllSet.Add(tempGameObject);
    }
    void CreateBinaryControlSetAt(Vector2 spaceRequired, GameObject whichController, int AffordanceIndex)
    {
        Vector2 startPoint = FindFirstEmpty(spaceRequired);
        GameObject tempGameObject = (GameObject)Instantiate(whichController);
        tempGameObject.transform.parent = this.transform;
        tempGameObject.transform.localPosition = (GUIGridPoints[startPoint + spaceRequired - Vector2.one].transform.localPosition + GUIGridPoints[startPoint].transform.localPosition) * .5f;
        tempGameObject.GetComponent<BinaryGUIHandler>().SetSprite(FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[AffordanceIndex]);
        OldControllSet.Add(tempGameObject);
        /*
        if (FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[AffordanceIndex].name == Affordances.Names.OnOff)
        {
            AssignOnOffButton(tempGameObject.GetComponentInChildren<BinaryButtonHandler>());
        }
         */
    }
    void CreateOneDimControlSetAt(Vector2 spaceRequired, GameObject whichController, int AffordanceIndex)
    {
        Vector2 startPoint = FindFirstEmpty(spaceRequired);
        GameObject tempGameObject = (GameObject)Instantiate(whichController);
        tempGameObject.transform.parent = this.transform;
        tempGameObject.transform.localPosition = (GUIGridPoints[startPoint + spaceRequired - Vector2.one].transform.localPosition + GUIGridPoints[startPoint].transform.localPosition) * .5f;
        tempGameObject.transform.localPosition -= (0.1f * Vector3.forward);
        tempGameObject.SendMessage("SetAffordance", FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[AffordanceIndex]);
        OldControllSet.Add(tempGameObject);
    }
    GameObject ImportControlSet(Vector2 spaceRequired, GameObject whichController)
    {
        Vector2 startPoint = FindFirstEmpty(spaceRequired);
        GameObject tempGameObject = (GameObject)Instantiate(whichController);
        tempGameObject.transform.parent = this.transform;
        tempGameObject.transform.localPosition = (GUIGridPoints[startPoint + spaceRequired - Vector2.one].transform.localPosition + GUIGridPoints[startPoint].transform.localPosition) * .5f;
     //   tempGameObject.transform.localPosition = (GUIGridPoints[new Vector2(2, 2)].transform.localPosition + GUIGridPoints[new Vector2(1, 2)].transform.localPosition) / 2;
        OldControllSet.Add(tempGameObject);
        return tempGameObject;
    }

    void AssignHandlers(GameObject controller, string name, List<Affordances> assigned)
    {
        // Assign each control component to the related affordance
        for (int i = 0; i < FunctionalityMenuHandler.SelectedDevice.ListOfAffordances.Count; i++)
        {
            var tempController = controller.transform.FindChild(name + "_" + FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i].name);
            if (tempController)
            {
                if (FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i].Type == Affordances.Types.Binary)
                {
                    tempController.GetComponent<BinaryGUIHandler>().SetSprite(FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i]);
                }
                else if (FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i].Type == Affordances.Types.OneDimensional)
                {
                    tempController.SendMessage("SetAffordance", FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i]);
                }
                assigned.Add(FunctionalityMenuHandler.SelectedDevice.ListOfAffordances[i]);
            }
            
        }
    }
    
    Vector2 FindFirstEmpty(Vector2 spaceRequired)
    {
        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                if (isFitable(new Vector2(i, j), spaceRequired))
                {
                    return (new Vector2(i, j));
                }
            }
        }
        return Vector2.zero;
    }

    bool isFitable(Vector2 where, Vector2 spaceRequired)
    {
        if (!GUIGridPoints[where].isOccupied)
        {
            if (where.x + spaceRequired.x <= GridSize.x)
            {
                if (where.y + spaceRequired.y <= GridSize.y)
                {
                    for (int p = (int)where.x; p < where.x + spaceRequired.x; p++)
                    {
                        for (int q = (int)where.y; q < where.y + spaceRequired.y; q++)
                        {
                            if (GUIGridPoints[new Vector2(p, q)].isOccupied)
                            {
                                return false;
                            }
                        }
                    }
                    for (int p = (int)where.x; p < where.x + spaceRequired.x; p++)
                    {
                        for (int q = (int)where.y; q < where.y + spaceRequired.y; q++)
                        {
                            GUIGridPoints[new Vector2(p, q)].SetToFull();
                        }
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }
    void EmptyGrid()
    {
        EmptyAllGridPoints();
    }
    #endregion
    // !!!
    public void ShowControlSet()
    {
        // !!!
        DestroyOldControlSet();
        EmptyGrid();
        CreateControllerSets();
        // !!!
        
    }

    void ChangeRowCol(int Row, int Col)
    {
        CurrentRow = Row;
        CurrentColoumn = Col;
        LayoutStartingPoint.transform.localPosition = LayoutStartingPoint_InitialPosition + (VerticalGap * Vector3.down * Row) + (HorizontalGap * Vector3.right * Col);
    }
    void GoToNextRow()
    {
        CurrentRow++;
        CurrentColoumn = -1;
        LayoutStartingPoint.transform.localPosition = LayoutStartingPoint_InitialPosition + (VerticalGap * Vector3.down * CurrentRow) + (0.5f * HorizontalGap * Vector3.right * CurrentColoumn);
    }
    void GotoMiddleOfNextRow()
    {
        CurrentRow++;
        CurrentColoumn = 0;
        LayoutStartingPoint.transform.localPosition = LayoutStartingPoint_InitialPosition + (VerticalGap * Vector3.down * CurrentRow) + (0.5f * HorizontalGap * Vector3.right * CurrentColoumn);
    }
    void GoToNextColoumn()
    {
        LayoutStartingPoint.transform.localPosition = LayoutStartingPoint_InitialPosition + (VerticalGap * Vector3.down * CurrentRow) + (0.5f * HorizontalGap * Vector3.right);
    }

    void DestroyOldControlSet()
    {
        for (int i = 0; i < OldControllSet.Count; i++)
        {
            Destroy((GameObject)OldControllSet[i]);
        }
    }

    void OnTapPanel(TapGesture gesture)
    {
        if (gesture.Selection == this.gameObject)
        {
            Sub_FunctionalityMenuHandler.GetDrawerOut();
        }
    }

    public void BringDrawerIn()
    {
        Comp_Animator.SetBool("GetIn", true);
    }

    public void GetDrawerOut()
    {
        Comp_Animator.SetBool("GetIn", false);
    }

}
