using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LifeHackerUtility.HotKeyWinApiWrapper;
using System.Windows;
using System.Diagnostics;

namespace LifeHackerUtility.LogicalClass
{
  class HotKeyManager
  {
    #region fields
    Window window;
    List<HotKeyCarryClass> hotkeys;
    List<HotKey> hotkeysListeners;
    StorageManager storageManager;
    #endregion

    #region properties
    private Window Window
    {
      get { return window; }
      set { window = value; }
    }
    private List<HotKeyCarryClass> Hotkeys
    {
      get { return hotkeys; }
      set { hotkeys = value; }
    }
    private List<HotKey> HotkeysListeners
    {
      get { return hotkeysListeners; }
      set { hotkeysListeners = value; }
    }
    private StorageManager StorageManager
    {
      get { return storageManager; }
      set { storageManager = value; }
    }
    #endregion

    #region constructors

    public HotKeyManager(Window window, StorageManager storageManager)
    {
      this.window = window;
      this.storageManager = storageManager;
      AdditionalInit();
      BoundEventHandlers();
    }

    private void AdditionalInit()
    {
      this.hotkeys = new List<HotKeyCarryClass>();
      this.hotkeysListeners = new List<HotKey>();

      foreach (HotKeyCarryClass hotkey in storageManager.LoadHotKeys())
      {
        //restore saved hotkeys
        RestoreHotKey(hotkey);
      }

    }

    private void BoundEventHandlers()
    {

    }

    #endregion

    #region methods

    public void RestoreHotKey(HotKeyCarryClass hotkey)
    {
      ModifierKeys mkey = ModifierKeys.None;
      foreach (ModifierKeys key in hotkey.ModifiersKeys) mkey |= key;

      Keys rkey = Keys.None;
      foreach (Keys key in hotkey.Keys) rkey |= key;

      //create hotkey listener and sets action
      HotKey hotkeyListener = new HotKey(hotkey.Id, mkey, rkey, window);
      hotkeyListener.HotKeyPressed += (k) =>
      {
        Process.Start(hotkey.PathToFile);
      };

      hotkeys.Add(hotkey);
      hotkeysListeners.Add(hotkeyListener);
    }

    public void AddHotKey(string id, List<Keys> hotkey, string pathToFile)
    {
      HotKeyCarryClass specifiedHotkey = hotkeys.Find((hotKeyObject) => { return hotKeyObject.Id == id; });
      HotKeyCarryClass hotkeyToStorage = specifiedHotkey;
      if (specifiedHotkey == null)
      {
        HotKeyCarryClass newHotKey = new HotKeyCarryClass(id, hotkey, pathToFile);
        hotkeyToStorage = newHotKey;

        ModifierKeys mkey = ModifierKeys.None;
        foreach (ModifierKeys key in newHotKey.ModifiersKeys) mkey |= key;

        Keys rkey = Keys.None;
        foreach (Keys key in newHotKey.Keys) rkey |= key;

        //create hotkey listener and sets action
        HotKey hotkeyListener = new HotKey(id, mkey, rkey, window);
        hotkeyListener.HotKeyPressed += (k) =>
        {
          Process.Start(pathToFile);
        };

        hotkeys.Add(newHotKey);
        hotkeysListeners.Add(hotkeyListener);
      }
      else
      {
        //sets new hotkey
        specifiedHotkey.SetNewHotkeys(hotkey);
        //sets path to file
        specifiedHotkey.PathToFile = pathToFile;

        //removes original hotkey listener
        HotKey originalListener = hotkeysListeners.Find((listener) => { return listener.Id == id; });
        hotkeysListeners.Remove(originalListener);
        originalListener.Dispose();

        //create new hotkey listener and add it into list
        ModifierKeys mkey = ModifierKeys.None;
        foreach (ModifierKeys key in specifiedHotkey.ModifiersKeys) mkey |= key;
        Keys rkey = Keys.None;
        foreach (Keys key in specifiedHotkey.Keys) rkey |= key;
        HotKey newListener = new HotKey(id, mkey, rkey, window);
        newListener.HotKeyPressed += (k) =>
        {
          Console.Beep();
          //Process.Start(pathToFile);
        };
        hotkeysListeners.Add(newListener);
      }
      storageManager.AddHotKey(hotkeyToStorage);
    }

    public void RemoveHotKey(string id)
    {
      //remove from manager
      hotkeys.RemoveAll((hotkeyObject) => { return hotkeyObject.Id == id; });
      HotKey selectedListener = hotkeysListeners.Find((listener) => { return listener.Id == id; });
      if (selectedListener != null)
      {
        selectedListener.Dispose();
        hotkeysListeners.Remove(selectedListener);
      }

      storageManager.DeleteHotKey(id);
    }

    #endregion
  }


}
