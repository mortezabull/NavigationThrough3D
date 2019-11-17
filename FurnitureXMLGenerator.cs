using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

public class FurnitureXMLGenerator
{
    public GameObject[] devices;

    public FurnitureXMLGenerator()
    {
        devices = GameObject.FindGameObjectsWithTag("Furniture");
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode bodyNode = xmlDoc.CreateElement("body");
        xmlDoc.AppendChild(bodyNode);

        XmlNode defineRoomNode = xmlDoc.CreateElement("define_room");
        ////////////////////////////
        XmlNode paramNode1 = xmlDoc.CreateElement("param");
        XmlAttribute attributeValue = xmlDoc.CreateAttribute("value");
        XmlAttribute attributeName = xmlDoc.CreateAttribute("name");
        attributeValue.Value = "ROOM";
        attributeName.Value = "guid";

        paramNode1.Attributes.Append(attributeValue);
        paramNode1.Attributes.Append(attributeName);
        defineRoomNode.AppendChild(paramNode1);
        //////////////////////////////////
        XmlNode paramNode2 = xmlDoc.CreateElement("param");
        XmlAttribute attributeValue2 = xmlDoc.CreateAttribute("value");
        XmlAttribute attributeName2 = xmlDoc.CreateAttribute("name");
        attributeValue2.Value = "ROOM";
        attributeName2.Value = "description";

        paramNode2.Attributes.Append(attributeValue2);
        paramNode2.Attributes.Append(attributeName2);
        defineRoomNode.AppendChild(paramNode2);
        //////////////////////////////////////////////////////////
        XmlNode paramNode3 = xmlDoc.CreateElement("param");
        XmlAttribute attributeValue3 = xmlDoc.CreateAttribute("value");
        XmlAttribute attributeName3 = xmlDoc.CreateAttribute("name");
        attributeValue3.Value = "0.0";
        attributeName3.Value = "startingx";

        paramNode3.Attributes.Append(attributeValue3);
        paramNode3.Attributes.Append(attributeName3);
        defineRoomNode.AppendChild(paramNode3);
        //////////////////////////////////////
        XmlNode paramNode4 = xmlDoc.CreateElement("param");
        XmlAttribute attributeValue4 = xmlDoc.CreateAttribute("value");
        XmlAttribute attributeName4 = xmlDoc.CreateAttribute("name");
        attributeValue4.Value = "0.0";
        attributeName4.Value = "startingy";

        paramNode4.Attributes.Append(attributeValue4);
        paramNode4.Attributes.Append(attributeName4);
        defineRoomNode.AppendChild(paramNode4);
        //////////////////////////////////////
        XmlNode paramNode5 = xmlDoc.CreateElement("param");
        XmlAttribute attributeValue5 = xmlDoc.CreateAttribute("value");
        XmlAttribute attributeName5 = xmlDoc.CreateAttribute("name");
        attributeValue5.Value = "0.0";
        attributeName5.Value = "startingz";

        paramNode5.Attributes.Append(attributeValue5);
        paramNode5.Attributes.Append(attributeName5);
        defineRoomNode.AppendChild(paramNode5);
        //////////////////////////////////////
        XmlNode paramNode6 = xmlDoc.CreateElement("param");
        XmlAttribute attributeValue6 = xmlDoc.CreateAttribute("value");
        XmlAttribute attributeName6 = xmlDoc.CreateAttribute("name");
        attributeValue6.Value = "0.0";
        attributeName6.Value = "RotateAroundZAxis";

        paramNode6.Attributes.Append(attributeValue6);
        paramNode6.Attributes.Append(attributeName6);
        defineRoomNode.AppendChild(paramNode6);
        //////////////////////////////////////
        XmlNode paramNode7 = xmlDoc.CreateElement("param");
        XmlAttribute attributeValue7 = xmlDoc.CreateAttribute("value");
        XmlAttribute attributeName7 = xmlDoc.CreateAttribute("name");
        attributeValue7.Value = "http://192.168.0.112/AMCo/Room small";
        attributeName7.Value = "URLBase";

        paramNode7.Attributes.Append(attributeValue7);
        paramNode7.Attributes.Append(attributeName7);
        defineRoomNode.AppendChild(paramNode7);
        //////////////////////////////////////
        XmlNode paramNode8 = xmlDoc.CreateElement("param");
        XmlAttribute attributeValue8 = xmlDoc.CreateAttribute("value");
        XmlAttribute attributeName8 = xmlDoc.CreateAttribute("name");
        attributeValue8.Value = "http://192.168.0.112/AMCo/Room small";
        attributeName8.Value = "ModelURL";

        paramNode8.Attributes.Append(attributeValue8);
        paramNode8.Attributes.Append(attributeName8);
        defineRoomNode.AppendChild(paramNode8);
        //////////////////////////////////////

        XmlNode roomNode = xmlDoc.CreateElement("room");
        XmlAttribute idAtt = xmlDoc.CreateAttribute("id");
        idAtt.Value = "407";
        /*DEVICE*******************************************************************************
         * ***********************************************************************************/
        int UUID = 1;
        foreach (GameObject device in devices)
        {
            XmlNode serviceNode = xmlDoc.CreateElement("service");
            ////////////////////////////
            XmlNode paramTemp1 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp = xmlDoc.CreateAttribute("name");
            attributeTemp.Value = (UUID++).ToString();
            attributeNTemp.Value = "guid";

            paramTemp1.Attributes.Append(attributeTemp);
            paramTemp1.Attributes.Append(attributeNTemp);
            serviceNode.AppendChild(paramTemp1);
            //////////////////////////////////
            XmlNode paramTemp2 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp2 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp2 = xmlDoc.CreateAttribute("name");
            //attributeTemp2.Value = device.name;
            //???
            attributeTemp2.Value = device.name;
            //attributeTemp2.Value = device.GetComponent<Device>().Name;
            //???
            attributeNTemp2.Value = "description";

            paramTemp2.Attributes.Append(attributeTemp2);
            paramTemp2.Attributes.Append(attributeNTemp2);
            serviceNode.AppendChild(paramTemp2);
            //////////////////////////////////////////////////////////
            XmlNode paramTemp3 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp3 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp3 = xmlDoc.CreateAttribute("name");
            attributeTemp3.Value = device.transform.position.x.ToString();
            attributeNTemp3.Value = "startingx";

            paramTemp3.Attributes.Append(attributeTemp3);
            paramTemp3.Attributes.Append(attributeNTemp3);
            serviceNode.AppendChild(paramTemp3);
            //////////////////////////////////////
            XmlNode paramTemp4 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp4 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp4 = xmlDoc.CreateAttribute("name");
            attributeTemp4.Value = device.transform.position.y.ToString();
            attributeNTemp4.Value = "startingy";

            paramTemp4.Attributes.Append(attributeTemp4);
            paramTemp4.Attributes.Append(attributeNTemp4);
            serviceNode.AppendChild(paramTemp4);
            //////////////////////////////////////
            XmlNode paramTemp5 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp5 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp5 = xmlDoc.CreateAttribute("name");
            attributeTemp5.Value = device.transform.position.z.ToString();
            attributeNTemp5.Value = "startingz";

            paramTemp5.Attributes.Append(attributeTemp5);
            paramTemp5.Attributes.Append(attributeNTemp5);
            serviceNode.AppendChild(paramTemp5);
            //////////////////////////////////////
            XmlNode paramTemp6 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp6 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp6 = xmlDoc.CreateAttribute("name");
            attributeTemp6.Value = device.transform.eulerAngles.x.ToString();
            attributeNTemp6.Value = "RotateAroundXAxis";

            paramTemp6.Attributes.Append(attributeTemp6);
            paramTemp6.Attributes.Append(attributeNTemp6);
            serviceNode.AppendChild(paramTemp6);
            //////////////////////////////////////
            XmlNode paramTemp7 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp7 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp7 = xmlDoc.CreateAttribute("name");
            attributeTemp7.Value = device.transform.eulerAngles.z.ToString();
            attributeNTemp7.Value = "RotateAroundYAxis";

            paramTemp7.Attributes.Append(attributeTemp7);
            paramTemp7.Attributes.Append(attributeNTemp7);
            serviceNode.AppendChild(paramTemp7);
            //////////////////////////////////////
            XmlNode paramTemp8 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp8 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp8 = xmlDoc.CreateAttribute("name");
            attributeTemp8.Value = device.transform.eulerAngles.y.ToString();
            attributeNTemp8.Value = "RotateAroundZAxis";

            paramTemp8.Attributes.Append(attributeTemp8);
            paramTemp8.Attributes.Append(attributeNTemp8);
            serviceNode.AppendChild(paramTemp8);
            //////////////////////////////////////
            XmlNode paramTemp9 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp9 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp9 = xmlDoc.CreateAttribute("name");
            attributeTemp9.Value = "http://192.168.0.112/AMCo/Room small";
            attributeNTemp9.Value = "URLBase";

            paramTemp9.Attributes.Append(attributeTemp9);
            paramTemp9.Attributes.Append(attributeNTemp9);
            serviceNode.AppendChild(paramTemp9);
            //////////////////////////////////////
            XmlNode paramTemp10 = xmlDoc.CreateElement("param");
            XmlAttribute attributeTemp10 = xmlDoc.CreateAttribute("value");
            XmlAttribute attributeNTemp10 = xmlDoc.CreateAttribute("name");
            attributeTemp10.Value = device.name;
            attributeNTemp10.Value = "ModelURL";

            paramTemp10.Attributes.Append(attributeTemp10);
            paramTemp10.Attributes.Append(attributeNTemp10);
            serviceNode.AppendChild(paramTemp10);
            //////////////////////////////////////

            roomNode.AppendChild(serviceNode);
        }



        bodyNode.AppendChild(defineRoomNode);
        bodyNode.AppendChild(roomNode);

        xmlDoc.Save(Application.persistentDataPath + "/RoomFurnitureXML.xml");
        Debug.Log("RoomFurnitureXML file Saved to: " + Application.persistentDataPath + "/RoomFurnitureXML.xml");
    }

}