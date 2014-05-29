using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LifeHackerUtility.HotKeyWinApiWrapper;
using System.Windows.Input;


namespace LifeHackerUtility.LogicalClass
{
  class HotKeyCarryClass
  {
    #region fields

    string id;
    List<ModifierKeys> modifierKeys = new List<ModifierKeys>();
    List<Keys> keys = new List<Keys>();
    string pathToFile;

    #endregion

    #region properties
    /// <summary>
    /// id of hotkey
    /// </summary>
    public string Id
    {
      get { return id; }
      set { id = value; }
    }
    /// <summary>
    /// list of modifier keys (shift, control, alt, win)
    /// </summary>
    public List<ModifierKeys> ModifiersKeys
    {
      get { return modifierKeys; }
      set { modifierKeys = value; }
    }
    /// <summary>
    /// other keys
    /// </summary>
    public List<Keys> Keys
    {
      get { return keys; }
      set { keys = value; }
    }
    /// <summary>
    /// path to file that will run on hotkey pressed
    /// </summary>
    public string PathToFile
    {
      get { return pathToFile; }
      set { pathToFile = value; }
    }
    #endregion

    #region constructors

    public HotKeyCarryClass(string id, List<Keys> keys, string pathToFile)
    {
      this.id = id;
      this.pathToFile = pathToFile;

      foreach (var key in keys)
      {
        if (IsModifierKey(key))
        {
          ModifiersKeys.Add(ConvertToModifierKey(key));
        }
        else
        {
          Keys.Add(key);
        }
      }

    }

    public HotKeyCarryClass(string id, List<ModifierKeys> modifierKeys, List<Keys> keys, string pathToFile)
    {
      this.id = id;
      this.modifierKeys = modifierKeys;
      this.keys = keys;
      this.pathToFile = pathToFile;
    }

    #endregion

    #region methods

    /// <summary>
    /// it sets new keys
    /// </summary>
    /// <param name="newKeys"></param>
    public void SetNewHotkeys(List<Keys> newKeys)
    {
      //clear original hotkeys
      Keys.Clear();
      ModifiersKeys.Clear();

      //set new hotkeys
      foreach (var key in newKeys)
      {
        if (IsModifierKey(key))
        {
          ModifiersKeys.Add(ConvertToModifierKey(key));
        }
        else
        {
          Keys.Add(key);
        }
      }
    }

    /// <summary>
    /// test if key is modifier key (shift,ctrl,alt,win)
    /// </summary>
    /// <param name="key"></param>
    /// <returns>True - it is modifier key, False - else</returns>
    private bool IsModifierKey(Keys key)
    {
      return
        key == HotKeyWinApiWrapper.Keys.LControlKey ||
        key == HotKeyWinApiWrapper.Keys.RControlKey ||
        key == HotKeyWinApiWrapper.Keys.Control ||
        key == HotKeyWinApiWrapper.Keys.ControlKey ||
        key == HotKeyWinApiWrapper.Keys.LShiftKey ||
        key == HotKeyWinApiWrapper.Keys.RShiftKey ||
        key == HotKeyWinApiWrapper.Keys.Shift ||
        key == HotKeyWinApiWrapper.Keys.ShiftKey ||
        key == HotKeyWinApiWrapper.Keys.LWin ||
        key == HotKeyWinApiWrapper.Keys.RWin;
    }

    /// <summary>
    /// It convert key to modifier key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private ModifierKeys ConvertToModifierKey(Keys key)
    {
      //ctrl
      if (key == HotKeyWinApiWrapper.Keys.LControlKey ||
        key == HotKeyWinApiWrapper.Keys.RControlKey ||
        key == HotKeyWinApiWrapper.Keys.Control ||
        key == HotKeyWinApiWrapper.Keys.ControlKey)
      {
        return ModifierKeys.Control;
      }

      //shift
      if (key == HotKeyWinApiWrapper.Keys.LShiftKey ||
        key == HotKeyWinApiWrapper.Keys.RShiftKey ||
        key == HotKeyWinApiWrapper.Keys.Shift ||
        key == HotKeyWinApiWrapper.Keys.ShiftKey)
      {
        return ModifierKeys.Shift;
      }

      //windows
      if (key == HotKeyWinApiWrapper.Keys.LWin ||
        key == HotKeyWinApiWrapper.Keys.RWin)
      {
        return ModifierKeys.Windows;
      }


      //this case not gonna happend
      return ModifierKeys.None;
    }

    #endregion
  }
}
