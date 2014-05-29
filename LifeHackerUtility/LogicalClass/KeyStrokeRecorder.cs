using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using LifeHackerUtility.LogicalClass;
using LifeHackerUtility.HotKeyWinApiWrapper;

namespace LifeHackerUtility
{
  class KeyStrokeRecorder
  {
    #region fields
    private object lockObject = new object();
    private List<Key> pressedKeys;
    private Dictionary<Key, bool> actuallyPressKeys;

    //control which events keyup and keydown
    //we actually listening
    private Control control;
    #endregion

    #region properties
    /// <summary>
    /// List of pressed keys
    /// </summary>
    private List<Key> PressedKeys
    {
      get
      {
        return pressedKeys;
      }
    }

    /// <summary>
    /// List of WinAPI compatibile keys which was pressed
    /// </summary>
    public List<Keys> WinAPICompatibileKeys
    {
      get { return ConvertKeysToWinAPICompatibileKeys(PressedKeys); }
    }

    #endregion

    #region events
    //event is raised when keystroke is completed
    public event Action<List<Keys>> KeyStrokeCompleted;
    //raise event
    private void OnKeyStrokeCompleted(List<Keys> keyStroke)
    {
      //invoke event
      if (KeyStrokeCompleted != null)
        KeyStrokeCompleted(keyStroke);
    }
    #endregion

    #region constructors
    //basic constructor
    public KeyStrokeRecorder(Control control)
    {
      this.pressedKeys = new List<Key>();
      this.actuallyPressKeys = new Dictionary<Key, bool>();
      this.control = control;
      //adds event handlers
      BoundEventHandlers();
    }
    //adds events handlers 
    private void BoundEventHandlers()
    {
      this.control.KeyUp += (sender, e) =>
      {
        Key input = (e.Key == Key.None) ? e.SystemKey : e.Key;
        KeyIsReleased(input);
      };

      this.control.KeyDown += (sender, e) =>
      {
        Key input = (e.Key == Key.None) ? e.SystemKey : e.Key;
        AddPressedKey(input);
      };
    }
    #endregion

    #region methods

    /// <summary>
    /// does add a new unique key into keystroke
    /// </summary>
    /// <param name="key"></param>
    public void AddPressedKey(Key key)
    {
      //lock acces to list of pressed keys
      lock (lockObject)
      {
        if (actuallyPressKeys.Count == 0) pressedKeys.Clear();
        if (!actuallyPressKeys.ContainsKey(key)) actuallyPressKeys.Add(key, true);
        if (!PressedKeys.Contains(key)) PressedKeys.Add(key);
      }
    }

    /// <summary>
    /// Does neccessery thinks when key is released.
    /// 
    /// </summary>
    /// <param name="key"></param>
    public void KeyIsReleased(Key key)
    {
      lock (lockObject)
      {
        actuallyPressKeys.Remove(key);
        if (actuallyPressKeys.Count == 0)
        {
          //all keys is released
          OnKeyStrokeCompleted(WinAPICompatibileKeys);
        }
      }
    }

    /// <summary>
    /// Gets human readable representation of keystroke.
    /// </summary>
    /// <returns></returns>
    public string GetTextRepresentationOfKeyStroke()
    {
      lock (lockObject)
      {
        return Library.GetTextRepresentationOfKeyStroke(WinAPICompatibileKeys);
      }
    }

    /// <summary>
    /// Converts WPF keys into WinApi compatibile keys.
    /// </summary>
    /// <param name="keys">list of keys</param>
    /// <returns>list of win api compatibile keys</returns>
    private List<Keys> ConvertKeysToWinAPICompatibileKeys(List<Key> keys)
    {
      List<Keys> winApiCompatibile = new List<Keys>();
      foreach (var key in keys)
      {
        winApiCompatibile.Add((Keys)KeyInterop.VirtualKeyFromKey(key));
      }
      return winApiCompatibile;
    }

    #endregion
  }
}
