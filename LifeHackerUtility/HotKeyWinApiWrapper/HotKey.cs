using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace LifeHackerUtility.HotKeyWinApiWrapper
{
  /// <summary>
  /// Class allow to register hotkeys in system 
  /// </summary>
  public sealed class HotKey : IDisposable
  {
    #region events

    /// <summary>
    /// event of pressed registred shortcut
    /// </summary>
    public event Action<HotKey> HotKeyPressed;

    /// <summary>
    /// Raise HotKeyPressed event
    /// </summary>
    private void OnHotKeyPressed()
    {
      if (HotKeyPressed != null)
        HotKeyPressed(this);
    }

    #endregion

    #region fields

    private readonly string publicId;
    private readonly int _id;
    private bool _isKeyRegistered;
    private readonly IntPtr _handle;

    private bool _disposed;

    #endregion

    #region properties

    public string Id
    {
      get { return publicId; }
    }

    public Keys Key { get; private set; }

    public ModifierKeys KeyModifier { get; private set; }

    #endregion

    #region constructors

    public HotKey(string id, ModifierKeys modifierKeys, Keys key, Window window)
      : this(id, modifierKeys, key, new WindowInteropHelper(window))
    {
      //tests if is requirment satisfied
      Contract.Requires(window != null);
    }

    public HotKey(string id, ModifierKeys modifierKeys, Keys key, WindowInteropHelper window)
      : this(id, modifierKeys, key, window.Handle)
    {
      //tests if is requirment satisfied
      Contract.Requires(window != null);
    }

    public HotKey(string id, ModifierKeys modifierKeys, Keys key, IntPtr windowHandle)
    {
      //tests if is requirment satisfied
      Contract.Requires(modifierKeys != ModifierKeys.None || key != Keys.None);
      Contract.Requires(windowHandle != IntPtr.Zero);

      Key = key;
      KeyModifier = modifierKeys;
      _id = GetHashCode();
      publicId = id;
      _handle = windowHandle;
      RegisterHotKey();
      ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;
    }

    #endregion

    #region destructors

    ~HotKey()
    {
      Dispose(false);
    }

    #endregion

    #region methods

    /// <summary>
    /// Add hotkey.
    /// </summary>
    public void RegisterHotKey()
    {
      if (Key == Keys.None)
        return;
      if (_isKeyRegistered)
        UnregisterHotKey();
      _isKeyRegistered = HotKeyWinApi.RegisterHotKey(_handle, _id, KeyModifier, Key);
      if (!_isKeyRegistered)
        throw new ApplicationException("Hotkey already in use");
    }

    /// <summary>
    /// Remove hotkey
    /// </summary>
    public void UnregisterHotKey()
    {
      _isKeyRegistered = !HotKeyWinApi.UnregisterHotKey(_handle, _id);
    }

    /// <summary>
    /// Dispose object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
    }

    /// <summary>
    /// Dispose object
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          ComponentDispatcher.ThreadPreprocessMessage -= ThreadPreprocessMessageMethod;
        }

        UnregisterHotKey();
        _disposed = true;
      }
    }

    /// <summary>
    /// Handler of key press. 
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="handled"></param>
    private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
    {
      if (!handled)
      {
        if (msg.message == HotKeyWinApi.WmHotKey
            && (int)(msg.wParam) == _id)
        {
          OnHotKeyPressed();
          handled = true;
        }
      }
    }

    #endregion

  }
}
