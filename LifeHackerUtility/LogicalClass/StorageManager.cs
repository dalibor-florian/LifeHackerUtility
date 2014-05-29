using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Input;
using LifeHackerUtility.HotKeyWinApiWrapper;

namespace LifeHackerUtility.LogicalClass
{
  class StorageManager
  {
    #region constants

    /// <summary>
    /// file that is used like app storage
    /// </summary>
    private const string STORAGE_FILE_NAME = "LifeHackerStorage.xml";

    //xml nodes
    private const string NODE_ROOT = "root";
    private const string NODE_HOTKEYS_LIST = "hotkeys";
    private const string NODE_HOTKEY = "hotkey";
    private const string NODE_COMMAND_KEYS = "command_keys";
    private const string NODE_KEYS = "keys";
    private const string NODE_COMMAND_KEY = "command_key";
    private const string NODE_KEY = "key";

    //hotkey atributes
    private const string ATTRIBUTE_ID = "id";
    private const string ATTRIBUTE_FILE_PATH = "file_path";

    #endregion

    #region fields

    string pathToStorageFile;
    XmlDocument storageXml;

    #endregion

    #region properties

    public XmlDocument StorageXml
    {
      get
      {

        if (storageXml == null)
        {
          storageXml = GetStorageXmlDocument();
        }

        return storageXml;
      }
    }

    #endregion

    #region constuctors

    /// <summary>
    ///  base constructor of storage manager
    /// </summary>
    public StorageManager()
    {
      //creates path to storage file
      pathToStorageFile =
        Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        STORAGE_FILE_NAME);

      //if storage file does not exist it creates it
      if (!File.Exists(pathToStorageFile))
      {
        InitializeStorage();
      }
    }

    #endregion

    #region methods

    public void AddHotKey(HotKeyCarryClass hotkey)
    {
      XmlNode hotkeyListElement = StorageXml.GetElementsByTagName(NODE_HOTKEYS_LIST)[0];
      XmlNode hotkeyNode = FindHotKeysNode(hotkey.Id);

      if (hotkeyNode != null)
      { //if there is already hotkey it removes it
        hotkeyListElement.RemoveChild(hotkeyNode);
      }

      //create completly new hotkey record
      XmlElement hotkeyElement = StorageXml.CreateElement(NODE_HOTKEY);
      hotkeyElement.SetAttribute(ATTRIBUTE_ID, hotkey.Id);
      hotkeyElement.SetAttribute(ATTRIBUTE_FILE_PATH, hotkey.PathToFile);

      //add all modifiers keys
      XmlElement modifiersKeysElement = StorageXml.CreateElement(NODE_COMMAND_KEYS);
      foreach (ModifierKeys mkey in hotkey.ModifiersKeys)
      {
        XmlElement mkeyNode = StorageXml.CreateElement(NODE_COMMAND_KEY);
        mkeyNode.InnerText = ((int)mkey).ToString();
        modifiersKeysElement.AppendChild(mkeyNode);
      }

      //add all keys
      XmlElement keysElement = StorageXml.CreateElement(NODE_KEYS);
      foreach (Keys key in hotkey.Keys)
      {
        XmlElement keyNode = StorageXml.CreateElement(NODE_KEY);
        keyNode.InnerText = ((int)key).ToString();
        keysElement.AppendChild(keyNode);
      }

      //adds keys into hotkey element
      hotkeyElement.AppendChild(modifiersKeysElement);
      hotkeyElement.AppendChild(keysElement);

      //adds hotkey element into list of hotkeys
      hotkeyListElement.AppendChild(hotkeyElement);
      
      //save changes
      StorageXml.Save(pathToStorageFile);
    }

    public void DeleteHotKey(string id)
    {
      XmlNode hotkeyListElement = StorageXml.GetElementsByTagName(NODE_HOTKEYS_LIST)[0];
      XmlNode selectedHotKey = FindHotKeysNode(id);

      //remove hotkey from storage
      hotkeyListElement.RemoveChild(selectedHotKey);

      //save changes
      StorageXml.Save(pathToStorageFile);
    }


    public List<HotKeyCarryClass> LoadHotKeys()
    {
      List<HotKeyCarryClass> hotkeysList = new List<HotKeyCarryClass>();

      XmlNode hotkeyListElement = StorageXml.GetElementsByTagName(NODE_HOTKEYS_LIST)[0];
      foreach (XmlNode node in hotkeyListElement.ChildNodes)
      {
        hotkeysList.Add(LoadHotKey(node));
      }
      
      return hotkeysList;
    }

    private HotKeyCarryClass LoadHotKey(XmlNode node)
    {
      string id = node.Attributes[ATTRIBUTE_ID].Value;
      string filePath = node.Attributes[ATTRIBUTE_FILE_PATH].Value;

      List<ModifierKeys> mkeysList = new List<ModifierKeys>();
      XmlNodeList modifiersKeys = node.ChildNodes[0].ChildNodes;
      for (int i = 0; i < modifiersKeys.Count; i++)
      {
        mkeysList.Add((ModifierKeys)Convert.ToInt32(modifiersKeys[i].InnerText));
      }

      List<Keys> keyList = new List<Keys>();
      XmlNodeList keys = node.ChildNodes[1].ChildNodes;
      for (int i = 0; i < keys.Count; i++)
      {
        keyList.Add((Keys)Convert.ToInt32(keys[i].InnerText));
      }

      return new HotKeyCarryClass(id, mkeysList, keyList, filePath);

    }


    private XmlNode FindHotKeysNode(string id)
    {
      XmlNode hotkeyListElement = StorageXml.GetElementsByTagName(NODE_HOTKEYS_LIST)[0];
      XmlNodeList nodeList = StorageXml.GetElementsByTagName(NODE_HOTKEY);

      foreach (XmlNode node in nodeList)
      {
        //it is hotkey what i looking for
        if (node.Attributes[ATTRIBUTE_ID].Value == id)
        {
          return node;
        }
      }

      return null;
    }


    private XmlDocument GetStorageXmlDocument()
    {
      try
      {
        XmlDocument xml = new XmlDocument();
        xml.Load(pathToStorageFile);
        return xml;
      }
      catch (Exception ex)
      {
        throw new Exception("Error occured during loading xml from storage file.", ex);
      }
    }

    private void InitializeStorage()
    {
      //creates document
      XmlDocument xml = new XmlDocument();

      //creates nodes
      XmlElement root = xml.CreateElement(NODE_ROOT);
      XmlElement hotkeys = xml.CreateElement(NODE_HOTKEYS_LIST);

      //add nodes into document
      root.AppendChild(hotkeys);
      xml.AppendChild(root);

      //save to storage file
      xml.Save(pathToStorageFile);
    }

    #endregion
  }
}
