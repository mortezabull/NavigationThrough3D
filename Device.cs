using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
//using MMT;
using ExtensionMethods;

[RequireComponent(typeof(MeshCollider))]
public class Device : MonoBehaviour
{
    #region Variable Declaration
    public string UUID;
    public string Name;

    public string BaseURL;
    public string ModelURL;
    [HideInInspector]
    public Vector3 Position;
    [HideInInspector]
    public Vector3 Orientation;
    public string[] Capabilities;
    public int OneDimensionalVariable = 10;

    public List<Affordances> ListOfAffordances;
    public Dictionary<Affordances.Names, Affordances> DictionaryOfAffordances;
    public Dictionary<Affordances.Names, FeedBackBase> DictionaryOfFeedBacks;
    public bool supportsMedia = false;

    public List<Affordances.MediaTypes> supportedMediaFormats;
    public bool isOn = false;

    internal TapRecognizer Comp_TapRecoginzer;
    internal LongPressRecognizer Comp_LongPressRecognizer;
    internal LongPressRecognizer Comp_LongPressRecognizer_Short;

    private FunctionalityMenuHandler Sub_FunctionalityMenuHandler;
    private LoadingAnimationHandler Sub_LoadingAnimationHandler;
    private Color initialColor;
    private Vector3 desiredPosition = Vector3.zero;
    private Quaternion desiredRotation = Quaternion.identity;
    private bool startMoving = false;
    private bool startRotation = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float changeColorDuration = 2;
    private float animationDuration = 1;
    private InputOutputManager Sub_DeviceConnectivityManager;
    private Lollypop Sub_Lollypop;
    //Adding Feedback
    private bool hasChildren = false;
    private GameObject instance_CompatibilitySign;
    private AudioSource Comp_AudioSource;

    private bool isPresentationPlaying = false;

    private Transform Child_MainBody;
    private Material[] initialMaterials;
    public Material initialMaterial;
    private Texture[] initialTextures;
    private Color[] initialColors;
    private string[] initialMaterialName;


    public GameObject TouchFeedBack;
    public BinaryButtonHandler Sub_OnOffButton;

    private GameObject Comp_TextMesh;

#if UNITY_PC
    MovieTexture movieTexture;
#endif
    //End of Adding Feedback to Model
    #endregion

    void Start()
    {
        #region Getting Initial Position and Rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        #endregion

        #region Initializing Important Variables
        desiredPosition = Vector3.zero;
        ListOfAffordances = new List<Affordances>();
        DictionaryOfAffordances = new Dictionary<Affordances.Names, Affordances>();
        DictionaryOfFeedBacks = new Dictionary<Affordances.Names, FeedBackBase>();
        supportedMediaFormats = new List<Affordances.MediaTypes>();
        hasChildren = (transform.childCount > 0);
        Child_MainBody = transform.FindChild("MainBody");

        Renderer Sub_Renderer = GetComponent<Renderer>() == null ? transform.GetChild(0).GetComponent<Renderer>() : GetComponent<Renderer>();

        if (Child_MainBody == null)
        {
            initialTextures = new Texture[Sub_Renderer.materials.Length];
            initialColors = new Color[Sub_Renderer.materials.Length];
            initialMaterialName = new string[Sub_Renderer.materials.Length];

            initialMaterials = Sub_Renderer.materials;
            initialMaterial = Sub_Renderer.material;

            for (int i = 0; i < Sub_Renderer.materials.Length; i++)
            {
                initialTextures[i] = Sub_Renderer.materials[i].mainTexture;
                initialMaterialName[i] = Sub_Renderer.materials[i].name;
                if (Sub_Renderer.materials[i].HasProperty("_Color"))
                {
                    initialColors[i] = Sub_Renderer.materials[i].color;
                }
            }
            if (Sub_Renderer.material.HasProperty("_Color"))
            {
                initialColor = Sub_Renderer.material.color;
            }
        }
        else
        {
            initialTextures = new Texture[Child_MainBody.GetComponent<Renderer>().materials.Length];
            initialColors = new Color[Child_MainBody.GetComponent<Renderer>().materials.Length];
            initialMaterialName = new string[Child_MainBody.GetComponent<Renderer>().materials.Length];

            initialMaterials = Child_MainBody.GetComponent<Renderer>().materials;
            initialMaterial = Child_MainBody.GetComponent<Renderer>().material;


            for (int i = 0; i < Child_MainBody.GetComponent<Renderer>().materials.Length; i++)
            {
                initialTextures[i] = Child_MainBody.GetComponent<Renderer>().materials[i].mainTexture;
                initialMaterialName[i] = Child_MainBody.GetComponent<Renderer>().materials[i].name;
                if (Child_MainBody.GetComponent<Renderer>().materials[i].HasProperty("_Color"))
                {
                    initialColors[i] = Child_MainBody.GetComponent<Renderer>().materials[i].color;
                }
            }

            if (Child_MainBody.GetComponent<Renderer>().material.HasProperty("_Color"))
            {
                initialColor = Child_MainBody.GetComponent<Renderer>().material.color;
            }
        }
        
        #endregion
        #region Finding Refrence to Important Scripts
        Sub_FunctionalityMenuHandler = GameObject.FindObjectOfType<FunctionalityMenuHandler>();
        Sub_LoadingAnimationHandler = GameObject.FindObjectOfType<LoadingAnimationHandler>();
        Sub_Lollypop = GameObject.FindObjectOfType<Lollypop>();
        Sub_DeviceConnectivityManager = GameObject.FindObjectOfType<InputOutputManager>();
        #endregion

        SetUpColliderAndAffordanceFeedback();

        RecognizingAffordances();

    }

    void Update()
    {
        if (startMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 0.1f);
        }
        if (startRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 10);
        }
    }

    void OnTap(TapGesture gesture)
    {
        if (gesture.Selection == gameObject)
        {
            if (!ModeSwitcher.isInTransition)
            {
                if (!GUISprites.isOnAGUIElement)
                {
                    if (!ChangePositionHandler.isInRepositioningMode)
                    {

                        if (!InputOutputManager.isUndergoingConnectivity)
                        {
                            //...Show device is touched
                            ShowSelectedState();

                            //If device only has one Affordance and it is a Binary one...
                            if (ListOfAffordances.Count == 1 && ListOfAffordances[0].Type == Affordances.Types.Binary)
                            {
                                //...Toggle it's binary affordance
                                ChangeAffordanceValue(new AffordanceInput(Affordances.Types.Binary, ListOfAffordances[0].name, (ListOfAffordances[0].value == 0) ? 1 : 0), true);
                                Invoke("ShowUnselectedState", changeColorDuration);
                            }
                            else
                            {
                                //...If not, bring up Functionality Drawer
                                if (Sub_FunctionalityMenuHandler)
                                    Sub_FunctionalityMenuHandler.BringUpMenu(this);
                            }
                            //...Revert device appearance back to it's original state

                        }
                        else
                        {
                            if (isCompatibleWithConnectivityType(InputOutputManager.ConnectingDeviceDictionary[InputOutputManager.ConnectingDevice]))
                            {
                                Sub_DeviceConnectivityManager.ConnectionAccepted(this);
                            }
                            else
                            {
                                Sub_DeviceConnectivityManager.ConnectionRefused(this);
                            }
                        }

                    }
                    else
                    {
                        ShowSelectedState();
                        ChangePositionHandler.SelectMovingObject(this);
                    }
                }
            }
        }
    }

    void OnLongPressShort(LongPressGesture gesture)
    {
        if (gesture.Selection == gameObject)
        {
            if (ListOfAffordances.Count == 1 && ListOfAffordances[0].Type == Affordances.Types.Binary)
            {
                if (!ModeSwitcher.isInTransition)
                {
                    if (!GUISprites.isOnAGUIElement)
                    {
                        if (!ChangePositionHandler.isInRepositioningMode)
                        {

                            if (!InputOutputManager.isUndergoingConnectivity)
                            {
                                //...Show device is touched
                                ShowSelectedState();

                                //...Bring up Functionality Drawer
                                Sub_FunctionalityMenuHandler.BringUpMenu(this);
                            }
                            else
                            {
                                if (isCompatibleWithConnectivityType(InputOutputManager.ConnectingDeviceDictionary[InputOutputManager.ConnectingDevice]))
                                {
                                    Sub_DeviceConnectivityManager.ConnectionAccepted(this);
                                }
                                else
                                {
                                    Sub_DeviceConnectivityManager.ConnectionRefused(this);
                                }
                            }

                        }
                        else
                        {
                            ShowSelectedState();
                            ChangePositionHandler.SelectMovingObject(this);
                        }
                    }
                }
            }
        }
    }

    void OnLongPressMover(LongPressGesture gesture)
    {

        if (gesture.Selection == this.gameObject)
        {

            if (!GUISprites.isOnAGUIElement)
            {

                MainDrawerHandler.CloseAllOpenDrawers();
                ShowSelectedState();
                ChangePositionHandler.SelectMovingObject(this);

            }
        }
    }

    IEnumerator ChangeColor(Color whichColor)
    {

        float elapsedTime = 0.0f;
        while (elapsedTime < changeColorDuration)
        {
            if (Child_MainBody == null)
            {
                if (GetComponent<Renderer>())
                {
                    GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, whichColor, (elapsedTime / changeColorDuration));
                }
            }
            else
            {
                if (Child_MainBody.GetComponent<Renderer>().material.HasProperty("_Color"))
                {
                    Child_MainBody.GetComponent<Renderer>().material.color = Color.Lerp(Child_MainBody.GetComponent<Renderer>().material.color, whichColor, (elapsedTime / changeColorDuration));
                }
            }

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }

    public void ShowSelectedState()
    {
        TouchFeedBack.GetComponent<ParticleSystem>().enableEmission = true;
        TouchFeedBack.GetComponent<FeedBackParticle>().IsActive = true;
        Resources.UnloadUnusedAssets();
    }

    public void ShowUnselectedState()
    {
        GetComponent<Renderer>().materials[0].color = Color.white;
        if (instance_CompatibilitySign != null)
        {
            Destroy(instance_CompatibilitySign);
        }
  
        if (Child_MainBody == null)
        {
            GetComponent<Renderer>().material = initialMaterial;
        }
        else
        {
            Child_MainBody.GetComponent<Renderer>().material = initialMaterial;
        }
        TouchFeedBack.GetComponent<ParticleSystem>().enableEmission = false;
        TouchFeedBack.GetComponent<FeedBackParticle>().IsActive = false;
    }

    #region Changing Position and Rotation

    public void GetMovement(Vector2 movement, bool isStationary)
    {
        desiredPosition = Vector3.zero;
        //Converting Pixel delta move to Centimeters
        movement.x = 20 * movement.x.Centimeters();
        movement.y = 20 * movement.y.Centimeters();

        desiredPosition = ((0.005f) * movement.y * transform.right) + ((-0.005f) * movement.x * transform.forward);

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, -5, 5);
        desiredPosition.z = Mathf.Clamp(desiredPosition.z, -5, 5);
        if (Application.platform == RuntimePlatform.Android)
        {
            desiredPosition *= 3;
        }
        desiredPosition += transform.position;

        desiredPosition.y = transform.position.y;


        startMoving = true;

    }

    
    public void GetHeight(Vector2 movement, bool isStatinary)
    {
        desiredPosition = Vector3.zero;
        //Converting Pixel delta move to Centimeters
        movement.y = 20 * movement.y.Centimeters();

        desiredPosition = ((0.005f) * movement.y * transform.up);

        desiredPosition.y = Mathf.Clamp(desiredPosition.y, -5, 5);

        if (Application.platform == RuntimePlatform.Android)
        {
            desiredPosition *= 3;
        }

        desiredPosition += transform.position;
        startMoving = true;
    }
    

    public void GetRotation(Vector2 movement, bool isStationary)
    {
        movement.x = movement.x / Screen.width;
        desiredRotation = Quaternion.Euler((movement.x * transform.up));
        desiredRotation *= transform.rotation;
        startRotation = true;
    }

    public void SetNewInitialPosition()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void ResetPositionAndRotation()
    {
        desiredPosition = initialPosition;
        desiredRotation = initialRotation;
        StartCoroutine("ResetTransform");
    }

    public IEnumerator MoveToPosition(Vector3 newLocalTarget)
    {
        //initiating elapsed time for coroutine to 0.0f
        float elapsedTime = 0.0f;
        //initiating startingPosition to current postion of joystick sprite
        Vector3 startingPosition = transform.localPosition;

        while (elapsedTime < animationDuration)
        {
            //moving joystick to it's initial position bit by bit
            transform.localPosition = Vector3.Lerp(startingPosition, newLocalTarget, (elapsedTime / animationDuration));

            elapsedTime += Time.deltaTime;
            //waiting until end of frame for next movement step
            yield return new WaitForEndOfFrame();
        }
        //movement done
        yield return 0;
    }

    public IEnumerator AlignToRotation(Quaternion newLocalRotation)
    {
        //initiating elapsed time for coroutine to 0.0f
        float elapsedTime = 0.0f;
        //initiating startingPosition to current postion of joystick sprite
        Quaternion startingRotation = transform.rotation;

        while (elapsedTime < animationDuration)
        {
            //moving joystick to it's initial position bit by bit
            transform.rotation = Quaternion.Lerp(startingRotation, newLocalRotation, (elapsedTime / animationDuration));

            elapsedTime += Time.deltaTime;
            //waiting until end of frame for next movement step
            yield return new WaitForEndOfFrame();
        }
        //movement done
        yield return 0;
    }

    #endregion

    void RecognizingAffordances()
    {
        string tempString = "";
        string tempString2 = "";

        for (int i = 0; i < ListOfAffordances.Count; i++)
        {
            ListOfAffordances[i] = new Affordances();
        }

        Affordances newAffordance;
        for (int i = 0; i < Capabilities.Length; i++)
        {
            newAffordance = new Affordances();
            tempString = Capabilities[i];
            tempString2 = tempString.Substring(tempString.IndexOf(":") + 1, tempString.Length - tempString.IndexOf(":") - 1);

            tempString = tempString.Substring(0, tempString.IndexOf(":"));


            if (tempString2 != Affordances.Types.Media.ToString())
            {

                for (int j = 0; j < Enum.GetNames(typeof(Affordances.Names)).Length; j++)
                {
                    if (tempString == Enum.GetNames(typeof(Affordances.Names))[j])
                    {
                        newAffordance.name = (Affordances.Names)(j);
                        continue;
                    }
                }

                for (int j = 0; j < Enum.GetNames(typeof(Affordances.Types)).Length; j++)
                {
                    if (tempString2 == Enum.GetNames(typeof(Affordances.Types))[j])
                    {
                        newAffordance.Type = (Affordances.Types)(j);
                        continue;
                    }
                }
                newAffordance.value = SetInitialValue(newAffordance.name, newAffordance.Type);
            }
            else
            {
                newAffordance.Type = Affordances.Types.Media;
                newAffordance.name = Affordances.Names.MediaSupport;
                GetMediaFormats(Capabilities[i].Substring(0, Capabilities[i].IndexOf(':')));
            }
            ListOfAffordances.Add(newAffordance);
            DictionaryOfAffordances.Add(newAffordance.name, newAffordance);
            InitializeFeedBackDictionary(newAffordance);

        }

        #region Initializing Each Affordance with a Value of 0. except for Mute and Volume affordances.
        for (int i = 0; i < DictionaryOfAffordances.Count; i++)
        {
            if (ListOfAffordances[i].name != Affordances.Names.MediaSupport)
            {
                switch (ListOfAffordances[i].name)
                {
                    case Affordances.Names.Mute:
                        {
                            InitializeFeedBack(ListOfAffordances[i], 1);
                            break;
                        }
                    case Affordances.Names.Volume:
                        {
                            InitializeFeedBack(ListOfAffordances[i], 25);
                            break;
                        }
                    default:
                        {
                            InitializeFeedBack(ListOfAffordances[i], 0);
                            break;
                        }
                }
            }

        }
        #endregion
    }

    void SetUpColliderAndAffordanceFeedback()
    {
        List<GameObject> OnFeedbacks = new List<GameObject>();
        List<GameObject> OffFeedbacks = new List<GameObject>();

        Transform temp_Transform = transform.FindChild("ColliderBody");
        GameObject temp_GameObject;

        if (temp_Transform != null)
        {
            //Set ColliderBody's Mesh as Mesh Collider
            if (transform.FindChild("ColliderBody").GetComponent<MeshFilter>())
            {
                this.GetComponent<MeshCollider>().sharedMesh = transform.FindChild("ColliderBody").GetComponent<MeshFilter>().mesh;
            }
            else if (transform.FindChild("ColliderBody").GetComponent<SkinnedMeshRenderer>())
            {
                this.GetComponent<MeshCollider>().sharedMesh = transform.FindChild("ColliderBody").GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }
            //Destroy Renderer of Collider body
            Destroy(transform.FindChild("ColliderBody").GetComponent<Renderer>());
        }
        temp_Transform = transform.FindChild("Light");
        // pair of "Light" and "Light.Target").
        if (temp_Transform != null && transform.FindChild("Light.Target"))
        {
            //... create a LightFeedback game object
            temp_GameObject = new GameObject("LighFeedback");
            //... Add a Light Component to it
            Light TempLight = temp_GameObject.AddComponent<Light>();
            //... Set Type of Light as Spotlight
            TempLight.type = LightType.Spot;
            //... Set Distance between Light and it's Target as the Light Range.
            TempLight.range = Vector3.Distance(temp_Transform.position, transform.FindChild("Light.Target").position);
            //... Set Intensity of Light to 8
            TempLight.intensity = 8;
            //... Set SpotAngle to 45 degrees
            TempLight.spotAngle = 45;
            //... Set Color of Light to Yellow
            TempLight.color = Color.yellow;
            //... Set Light FeedBack parent to this device
            temp_GameObject.transform.parent = this.transform;
            //... Set Position of this light to that of "Light" start point
            temp_GameObject.transform.localPosition = temp_Transform.transform.localPosition;
            //... Look at "Target" to Set light direction correctly
            temp_GameObject.transform.LookAt(transform.FindChild(temp_Transform.name + ".Target"));

            OnFeedbacks.Add(temp_GameObject);

        }

        temp_Transform = transform.FindChild("HaloR");
        if (temp_Transform != null)
        {
            //... create a LightFeedback game object
            temp_GameObject = (GameObject)Instantiate(Sub_Lollypop.HaloRedPrefab);
            //... Set Light FeedBack parent to this device
            temp_GameObject.transform.parent = this.transform;
            //... Set Position of this light to that of "Light" start point
            temp_GameObject.transform.localPosition = temp_Transform.transform.localPosition;
            //... Look at "Target" to Set light direction correctly
            temp_GameObject.transform.LookAt(transform.FindChild(temp_Transform.name + ".Target"));
            OffFeedbacks.Add(temp_GameObject);
        }

        temp_Transform = transform.FindChild("HaloG");
        if (temp_Transform != null)
        {
            //... create a LightFeedback game object
            temp_GameObject = (GameObject)Instantiate(Sub_Lollypop.HaloGreenPrefab);
            //... Set Light FeedBack parent to this device
            temp_GameObject.transform.parent = this.transform;
            //... Set Position of this light to that of "Light" start point
            temp_GameObject.transform.localPosition = temp_Transform.transform.localPosition;
            //... Look at "Target" to Set light direction correctly
            temp_GameObject.transform.LookAt(transform.FindChild(temp_Transform.name + ".Target"));
            OnFeedbacks.Add(temp_GameObject);
        }

        temp_Transform = transform.FindChild("RenderPlaneOn");
        if (temp_Transform != null)
        {
            OnFeedbacks.Add(temp_Transform.gameObject);
        }

        temp_Transform = transform.FindChild("RenderPlaneOff");
        if (temp_Transform != null)
        {
            OffFeedbacks.Add(temp_Transform.gameObject);
        }

        #region Adding LightFeedback to OnOff Feedback
        //Create OnOff Feedback for this device
        GameObject OnOffFeedback = new GameObject("OnOffFeedback");
        //Get a Refrence to it's OnOffHandler
        OnOffHandler Comp_OnOffHandler = OnOffFeedback.AddComponent<OnOffHandler>();
        //Make it a child of this device
        OnOffFeedback.transform.parent = this.transform;
        //... and set it's position to (0,0,0) of this device
        OnOffFeedback.transform.localPosition = Vector3.zero;
        //Set Affordance type of this FeedbackBase to "OnOff"
        Comp_OnOffHandler.thisFeedBackAffordance = Affordances.Names.OnOff;
        //initialize OnAssets (Just Light for now)
        Comp_OnOffHandler.OnAssets = new GameObject[OnFeedbacks.Count];

        //Adding lights to OnOffHandler
        for (int i = 0; i < OnFeedbacks.Count; i++)
        {
            Comp_OnOffHandler.OnAssets[i] = OnFeedbacks[i];
        }

        Comp_OnOffHandler.OffAssets = new GameObject[OffFeedbacks.Count];

        for (int i = 0; i < OffFeedbacks.Count; i++)
        {
            Comp_OnOffHandler.OffAssets[i] = OffFeedbacks[i];
        }

        if (transform.FindChild("ParticleSystem"))
        {
            GameObject temp_SpeedState = new GameObject("SpeedStateFeedback");
            SpeedStateHandler Comp_SpeedStateHandler = temp_SpeedState.AddComponent<SpeedStateHandler>();
            temp_SpeedState.transform.parent = this.transform;
            temp_SpeedState.transform.localPosition = Vector3.zero;
            Comp_SpeedStateHandler.thisFeedBackAffordance = Affordances.Names.SpeedState;

        }
        #endregion

    }
    
    public void GetMediaFormats(string MediaFormatsString)
    {
        supportsMedia = true;
        string tempString3 = MediaFormatsString;
        string tempString4 = tempString3;
        int numberOfSeperators = 0;
        for (int k = 0; k < tempString3.Length; k++)
        {
            if (tempString3[k] == '-')
            {
                numberOfSeperators++;
            }
        }
        for (int p = 0; p < numberOfSeperators + 1; p++)
        {
            if (p != numberOfSeperators)
            {
                tempString4 = tempString3.Substring(tempString3.IndexOf('.') + 1, tempString3.IndexOf('-') - 1);
                for (int j = 0; j < Enum.GetNames(typeof(Affordances.MediaTypes)).Length; j++)
                {
                    if (tempString4 == Enum.GetNames(typeof(Affordances.MediaTypes))[j])
                    {
                        supportedMediaFormats.Add((Affordances.MediaTypes)(j));
                    }
                }
            }
            else
            {
                tempString4 = tempString3.Substring(tempString3.IndexOf('.') + 1, tempString3.Length - 1);
                for (int j = 0; j < Enum.GetNames(typeof(Affordances.MediaTypes)).Length; j++)
                {
                    if (tempString4 == Enum.GetNames(typeof(Affordances.MediaTypes))[j])
                    {
                        supportedMediaFormats.Add((Affordances.MediaTypes)(j));
                    }
                }
            }
            tempString3 = tempString3.Remove(0, tempString3.IndexOf('-') + 1);
        }
    }

    public void ChangeAffordanceValue(AffordanceInput input, bool sendMessageToServer)
    {
        // I want to turn a group of lights on or off
        string groupName = Name.Contains("_sub") ? Name.Substring(0, Name.Length - 5) : Name;
        switch (input.InputType)
        {

            #region Binary Affordance
            case Affordances.Types.Binary:
                {

                    switch (input.InputName)
                    {
                        case Affordances.Names.OnOff:
                            {

                                if (Comp_AudioSource != null)
                                {
                                    if (Comp_AudioSource.isPlaying)
                                    {
                                        Comp_AudioSource.Stop();
                                    }
                                }

                                break;
                            }
                        case Affordances.Names.DoubleOnOff:
                            {

                                if (Comp_AudioSource != null)
                                {
                                    if (Comp_AudioSource.isPlaying)
                                    {
                                        Comp_AudioSource.Stop();
                                    }
                                }

                                break;
                            }

                        case Affordances.Names.DoorState:
                            {
                                if (GetComponent<Animator>())
                                {
                                    if (input.InputValue == 0)
                                    {
                                        GetComponent<Animator>().SetBool("isOpen", false);
                                    }
                                    else
                                    {
                                        GetComponent<Animator>().SetBool("isOpen", true);
                                    }
                                }
                                break;
                            }
                        case Affordances.Names.Elevation:
                            {
                                if (GetComponent<Animator>())
                                {
                                    if (input.InputValue == 1)
                                    {
                                        GetComponent<Animator>().SetBool("isUp", false);
                                    }
                                    else
                                    {
                                        GetComponent<Animator>().SetBool("isUp", true);
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
            #endregion

            #region One Dimensional Affordance
            case Affordances.Types.OneDimensional:
                {
                    switch (input.InputName)
                    {
                        case Affordances.Names.SpeedState:
                            {
                                if (GetComponent<Animator>())
                                {
                                    if (input.InputValue == 0)
                                    {
                                        GetComponent<Animator>().SetInteger("fanSpeed", 0);
                                    }
                                    else
                                    {
                                        GetComponent<Animator>().SetInteger("fanSpeed", (int)DictionaryOfAffordances[Affordances.Names.SpeedState].value);
                                    }
                                }
                                // speedState has a value between 0 and 5
                                if (input.InputValue < 5)
                                {
                                    if (DictionaryOfAffordances[Affordances.Names.OnOff].value == 0)
                                    {
                                        //TurnDeviceOnOrOff(1);
                                        TurnOnOrOff(1);
                                    }
                                    else
                                    {
                                        //TurnDeviceOnOrOff(0);
                                        TurnOnOrOff(0);
                                    }

                                }
                                break;
                            }
                        case Affordances.Names.ThermoState:
                            {
                                if (GetComponent<Animator>())
                                {
                                    if (input.InputValue == 0)
                                    {
                                        GetComponent<Animator>().SetInteger("fanSpeed", 0);
                                    }
                                    else
                                    {
                                        GetComponent<Animator>().SetInteger("fanSpeed", (int)DictionaryOfAffordances[Affordances.Names.SpeedState].value);
                                    }
                                }
                                if (input.InputValue < 100)
                                {
                                    if (DictionaryOfAffordances[Affordances.Names.OnOff].value == 0)
                                    {
                                        //TurnDeviceOnOrOff(1);
                                        TurnOnOrOff(1);
                                    }

                                    else
                                    {
                                        TurnOnOrOff(0);
                                    }

                                }
                                break;
                            }
                        case Affordances.Names.Volume:
                            {
                                if (Comp_AudioSource != null)
                                {
                                    if (Comp_AudioSource.isPlaying)
                                    {
                                        Comp_AudioSource.volume = Mathf.Clamp01(input.InputValue / 50f);
                                    }
                                }
                                break;
                            }


                    }
                    break;
                }
                #endregion

        }

        ShowInitialFeedBack(input.InputName, input.InputValue);
        // a beep sound is played as feedback to user action
        SoundFeedbackManager.PlayFeedbackSound(SoundFeedbackManager.FeedbackSoundTypes.Touch);
    }

    public void ShowInitialFeedBack(Affordances.Names WhichAffordance, float value)
    {
        DictionaryOfAffordances[WhichAffordance].value = value;
        if (DictionaryOfFeedBacks.ContainsKey(WhichAffordance))
        {
            if (DictionaryOfFeedBacks[WhichAffordance].gameObject.activeInHierarchy)
            {
                DictionaryOfFeedBacks[WhichAffordance].ChangeState(value);
            }
        }
    }

    void InitializeFeedBackDictionary(Affordances WhichAffordance)
    {
        if (transform.FindChild(WhichAffordance.name.ToString() + "Feedback"))
        {
            DictionaryOfFeedBacks.Add(WhichAffordance.name, transform.FindChild(WhichAffordance.name.ToString() + "Feedback").GetComponent<FeedBackBase>());
        }

    }

    //Initializing value of Affordances and show it with Feedback systems.

    void InitializeFeedBack(Affordances WhichAffordance, float value)
    {
        WhichAffordance.value = value;
        if (DictionaryOfFeedBacks.ContainsKey(WhichAffordance.name))
        {
            DictionaryOfFeedBacks[WhichAffordance.name].InitializeFeedBack(this);
        }
    }

    public void ShowCompatibility()
    {

        initialMaterial = GetComponent<Renderer>().material;
        if (Child_MainBody != null)
        {
            Child_MainBody.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/VertexAnimation_Custom");
            Child_MainBody.GetComponent<Renderer>().material.SetTexture("_Diffuse", initialMaterial.GetTexture(0));
            Child_MainBody.GetComponent<Renderer>().material.SetTexture("_Normals", initialMaterial.GetTexture(0));

        }
        else
        {
            if (GetComponent<Renderer>())
            {

                GetComponent<Renderer>().material = Resources.Load<Material>("Materials/VertexAnimation_Custom");
                GetComponent<Renderer>().material.SetTexture("_Diffuse", initialMaterial.mainTexture);
                GetComponent<Renderer>().material.SetTexture("_Normals", initialMaterial.mainTexture);

            }
        }
        Resources.UnloadUnusedAssets();
    }

    bool isCompatibleWithConnectivityType(Affordances.Names WhichAffordance)
    {
        if (WhichAffordance == Affordances.Names.InputConnect && DictionaryOfAffordances.ContainsKey(Affordances.Names.OutputConnect))
        {

            return true;
        }
        else if (WhichAffordance == Affordances.Names.OutputConnect && DictionaryOfAffordances.ContainsKey(Affordances.Names.InputConnect))
        {
            return true;
        }
        return false;
    }

    //Set initial value for all affordances. 
    int SetInitialValue(Affordances.Names affordanceName, Affordances.Types affordanceType)
    {
        if (true)
        {
            return 0;
        }
    }

    public void TurnOnOrOff(int turn)
    {

        if (DictionaryOfAffordances.ContainsKey(Affordances.Names.OnOff))
        {
            if (DictionaryOfAffordances[Affordances.Names.OnOff].value != turn)
            {
                if (FunctionalityMenuHandler.SelectedDevice)
                {
                    GameObject.FindObjectOfType<BinaryButtonHandler>().ChangeSprite();
                }
                ChangeAffordanceValue(new AffordanceInput(Affordances.Types.Binary, Affordances.Names.OnOff, turn), true);
                ShowInitialFeedBack(Affordances.Names.OnOff, turn);
            }
        }
        else if (DictionaryOfAffordances.ContainsKey(Affordances.Names.DoubleOnOff))
        {
            if (DictionaryOfAffordances[Affordances.Names.DoubleOnOff].value != turn)
            {
                if (FunctionalityMenuHandler.SelectedDevice)
                {
                    GameObject.FindObjectOfType<BinaryButtonHandler>().ChangeSprite();
                }
                ChangeAffordanceValue(new AffordanceInput(Affordances.Types.Binary, Affordances.Names.DoubleOnOff, turn), true);
                ShowInitialFeedBack(Affordances.Names.DoubleOnOff, turn);
            }
        }
        else if (DictionaryOfAffordances.ContainsKey(Affordances.Names.On))
        {
            ChangeAffordanceValue(new AffordanceInput(Affordances.Types.Binary, Affordances.Names.On, turn), true);
        }

    }

    //Coroutin uset to align camera with a point and a rotation
    public IEnumerator ResetTransform()
    {
        float time = 0.5f;
        float elapsedTime = 0.0f;
        Vector3 startingPosition = transform.localPosition;

        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, desiredPosition, (elapsedTime / time));

            // Rotations
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, (elapsedTime / time));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (!ChangePositionHandler.isInRepositioningMode)
        {
            ShowUnselectedState();
        }
        transform.localPosition = desiredPosition;
        transform.rotation = desiredRotation;
        yield return 0;
    }

}
 
