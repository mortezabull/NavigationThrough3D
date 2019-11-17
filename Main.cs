using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Text;

using System;

public sealed class Lollypop : MonoBehaviour
{
    #region Singleton Pattern Implemention
    private static readonly Lollypop instance = new Lollypop();
    private Lollypop() { }
    public static Lollypop Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region Variable Declaration
    public Color AmbientColor;
    [Range(0, 5)]
    public int WhichRoomToLoad = 1;
    internal string RoomName;

    public GameObject Text;

    GameObject gui;
    internal List<GameObject> deviceList;
    internal List<Device> deviceList_DeviceComponent;

    public GameObject TouchFeedBack;
    public GameObject SpeedFeedback;

    GameObject device;
    GameObject furniture;
    GameObject tempDevice;
    GameObject tempFurniture;
    GameObject Environment;
    GameObject Furniture;
    ModeSwitcher Sub_CameraSwitcher;
    public string LongPressEventName;
    public TextMesh TextMeshTester;
    public GameObject CompatibilityIndicator;
    private delegate void ChangeMaterialColor();
    private ChangeMaterialColor ChangeColorBackForAllDevices;
    internal Dictionary<string, Device> DictionaryOfIDDevicePair;
    internal List<GameObject> floors;
    internal Vector3 floorPosition = Vector3.zero;
    public string[] OverallModes;
    //public GameObject BoundingLayerBox;

    private TextAsset textAsset;
    public GameObject HaloRedPrefab;
    public GameObject HaloGreenPrefab;
    //Delay Betweeen Each Message in MiliSeconds
    [Range(0, 1000)]
    public int EachMessageDelay;

    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    #endregion

    void Start()
    {
     
        ChangeRoom();
        if (GameObject.FindObjectOfType<NewAutomationPre>())
        {
            GameObject.FindObjectOfType<NewAutomationPre>().PresentEvents();
        }
    }
        public void ChangeRoom()
        {
             #region Preventing Screen from Sleeping
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #endregion

        #region Initializing Scene
        if (Furniture)
        {
            Destroy(Furniture.gameObject);
        }
        if (Environment)
        {
            Destroy(Environment.gameObject);
        }

        Furniture = new GameObject("Furniture");
        Environment = new GameObject("Environment");
        Environment.transform.parent = this.transform;
        RenderSettings.ambientLight = AmbientColor;
    //    GameObject.FindObjectOfType<BirdEyeViewer>().camera.backgroundColor = AmbientColor;

        #endregion

        #region Initializing Variables
        floors = new List<GameObject>();
        deviceList = new List<GameObject>();
        deviceList_DeviceComponent = new List<Device>();
        DictionaryOfIDDevicePair = new Dictionary<string, Device>();

        #endregion

        #region Finding Refrence to Important Scripts
        Sub_CameraSwitcher = GameObject.FindObjectOfType<ModeSwitcher>();
        #endregion
        #region Loading Furnitures
        switch (WhichRoomToLoad)
        {
            case 0:
                {
                    textAsset = (TextAsset)Resources.Load("RoomFurnitureXML");
                    break;
                }
           
            default:
                {
                    Debug.Log("Check WhihRoomToLoad value!");
                    break;
                }
        }
        // build objects according to information in the XML file
        ConfigureFurnituresWithString(textAsset.text);

        #endregion

        #region Loading Devices from XML. If It's the first time program runs, we load devices from a preloaded XML. If not we load from last saved XML.
        try
        {
            switch (WhichRoomToLoad)
            {
                case 0:
                    {
                        Configure(Application.persistentDataPath + "/Room0XML.xml");
                        break;
                    }
                case 1:
                    {
                        Configure(Application.persistentDataPath + "/Room1XML.xml");
                        break;
                    }
                case 2:
                    {
                        Configure(Application.persistentDataPath + "/Room2XML.xml");
                        break;
                    }
                case 3:
                    {
                        Configure(Application.persistentDataPath + "/Room3XML.xml");
                        break;
                    }
                case 4:
                    {
                        Configure(Application.persistentDataPath + "/Room4XML.xml");
                        break;
                    }
                default:
                    {
                        Debug.Log("Check WhichRoomToLoad value!");
                        break;
                    }
            }
            //Configure(Application.persistentDataPath + "/Room" + WhichRoomToLoad + "XML");

            Debug.Log("Loaded from PersistentDataPath Folder");
        }
        catch
        {
            switch (WhichRoomToLoad)
            {
                case 0:
                    {
                        textAsset = (TextAsset)Resources.Load("RoomXML");
                        break;
                    }
                
                default:
                    {
                        Debug.Log("Check WhichRoomToLoad value!");
                        break;
                    }
            }

            ConfigureWithString(textAsset.text);

        }

        #endregion

        #region Saving Devices XML
        XMLGenarator tempGenerator = new XMLGenarator();
        #endregion

        #region [Test] Trying to Interpret Command XML
        try
        {
            ConfigCommandFile();
        }
        catch
        {
            Debug.Log("Couldn't Parse Command XML");
        }

        #endregion

        #region Calculating Floors mid position for BirdEyeCamera
        for (int f = 0; f < floors.Count; f++)
        {
            floorPosition += floors[f].transform.position;
        }
        floorPosition = floorPosition / (float)floors.Count;

        #endregion
    }


    public Dictionary<String, Device> ListOfDevices()
    {
        return DictionaryOfIDDevicePair;

    }
    

    Affordances.Types FindType(string TypeString)
    {
        for (int i = 0; i < Enum.GetNames(typeof(Affordances.Types)).Length; i++)
        {
            if (TypeString == Enum.GetNames(typeof(Affordances.Types))[i])
            {
                return (Affordances.Types)i;
            }

        }
        return Affordances.Types.Media;
    }

    Affordances.Names FindName(string NameString)
    {
        for (int i = 0; i < Enum.GetNames(typeof(Affordances.Names)).Length; i++)
        {
            if (NameString == Enum.GetNames(typeof(Affordances.Names))[i])
            {
                return (Affordances.Names)i;
            }

        }
        return Affordances.Names.OnOff;
    }

    #region Command File Operations

    void ConfigCommandFile()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("CommunicationProtocol");

        ParseCommandFile(textAsset.text);
    }

    void ParseCommandFile(string document)
    {
        XmlDocument configFile = new XmlDocument();
        XmlNodeList xmlnode;
        try
        {
            configFile.LoadXml(document);
        }
        catch
        {
            Debug.Log("Couldn't Load XML");
        }
        // Getting All Commands
        xmlnode = configFile.GetElementsByTagName("main");
        for (int i = 0; i < xmlnode[0].ChildNodes.Count; i++)
        {
            switch (xmlnode[0].ChildNodes[i].Name)
            {
                case "command":
                    {
                        InterpretCommand(xmlnode[0].ChildNodes[i]);

                        break;
                    }
                case "transaction":
                    {

                        break;
                    }
                case "undo":
                    {

                        break;
                    }
                case "redo":
                    {

                        break;
                    }
                case "subscribe":
                    {

                        break;
                    }
                case "unsubscribe":
                    {

                        break;
                    }
                case "notify":
                    {

                        break;
                    }

            }
        }
    }

    void InterpretCommand(XmlNode commandNode)
    {

        for (int i = 0; i < commandNode.ChildNodes.Count; i++)
        {
            //            Debug.Log(i + "th Element is: " + commandNode.ChildNodes[i].Name + " it's value is: " + commandNode.ChildNodes[i].InnerText);
        }

        //0th Element is MID

        //1st Element is UID

        //2nd Element is Key

        //3rd Element is Value

        //4th Element is Situation

        CreateCommand("command", commandNode.ChildNodes[0].InnerText, commandNode.ChildNodes[1].InnerText, commandNode.ChildNodes[2].InnerText, commandNode.ChildNodes[3].InnerText, commandNode.ChildNodes[4].InnerText);
    }

    string CreateCommand(string title, string mid, string uid, string key, string value, string situation)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode bodyNode = xmlDoc.CreateElement(title);

        XmlNode MIDNode = xmlDoc.CreateElement("mid");
        MIDNode.InnerText = mid;
        bodyNode.AppendChild(MIDNode);

        XmlNode UIDNode = xmlDoc.CreateElement("uid");
        UIDNode.InnerText = uid;
        bodyNode.AppendChild(UIDNode);

        XmlNode KeyNode = xmlDoc.CreateElement("key");
        KeyNode.InnerText = key;
        bodyNode.AppendChild(KeyNode);

        XmlNode ValueNode = xmlDoc.CreateElement("value");
        //ValueNode.InnerText = value;
        ValueNode.InnerText = CreateSubCommand("command", "M", "U", "K", "V", "S");
        bodyNode.AppendChild(ValueNode);

        XmlNode SituationNode = xmlDoc.CreateElement("situation");
        SituationNode.InnerText = situation;
        bodyNode.AppendChild(SituationNode);

        xmlDoc.AppendChild(bodyNode);

        xmlDoc.Save(Application.persistentDataPath + "/SimpleCommand.xml");

        return bodyNode.InnerText;

    }

    string CreateSubCommand(string title, string mid, string uid, string key, string value, string situation)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode bodyNode = xmlDoc.CreateElement(title);

        XmlNode MIDNode = xmlDoc.CreateElement("mid");
        MIDNode.InnerText = mid;
        bodyNode.AppendChild(MIDNode);

        XmlNode UIDNode = xmlDoc.CreateElement("uid");
        UIDNode.InnerText = uid;
        bodyNode.AppendChild(UIDNode);

        XmlNode KeyNode = xmlDoc.CreateElement("key");
        KeyNode.InnerText = key;
        bodyNode.AppendChild(KeyNode);

        XmlNode ValueNode = xmlDoc.CreateElement("value");
        ValueNode.InnerText = value;
        bodyNode.AppendChild(ValueNode);

        XmlNode SituationNode = xmlDoc.CreateElement("situation");
        SituationNode.InnerText = situation;
        bodyNode.AppendChild(SituationNode);

        xmlDoc.AppendChild(bodyNode);

        return bodyNode.InnerText;

    }

    #endregion
    public void RecieveMessageHandler(string Message)
    {
        if (Message.Contains("Device"))
        {
            ConfigureOneDevice(Message.Substring("Add Device".Length, Message.Length - "Add Device".Length));
        }
        else if (Message.Contains("Furniture"))
        {
            ConfigureFurnituresWithString(Message);
        }
        else if (Message.Contains("Change"))
        {
            WhichRoomToLoad = Int32.Parse(Message.Substring(Message.Length - 1, 1));
            ChangeRoom();
        }
    }
}


