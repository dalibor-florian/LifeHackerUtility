using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using LifeHackerUtility.HotKeyWinApiWrapper;
using LifeHackerUtility.LogicalClass;
using LifeHackerUtility.Windows;
using Microsoft.Win32;

namespace LifeHackerUtility.Components
{
  /// <summary>
  /// Interaction logic for HotKeyRow.xaml
  /// </summary>
  public partial class HotKeyRow : UserControl
  {

    #region fields
    string id = null;
    List<Keys> hotkey;
    string pathToFile;
    #endregion

    #region properties
    /// <summary>
    /// indicates if hotkey is already saved
    /// </summary>
    public bool IsHotKeySaved
    {
      get { return !string.IsNullOrEmpty(id); }
    }
    /// <summary>
    /// Unique id of hotkey
    /// </summary>
    public string Id
    {
      get { return id; }
      set { id = value; }
    }
    /// <summary>
    /// list of keys in hotkey
    /// </summary>
    public List<Keys> Hotkey
    {
      get { return hotkey; }
      set
      {
        hotkey = value;
        if (hotkey != null &&
            !string.IsNullOrEmpty(pathToFile))
        {
          OnHotKeyUpdate();
        }
      }
    }
    /// <summary>
    /// path to file which has to
    /// be run 
    /// </summary>
    public string PathToFile
    {
      get { return pathToFile; }
      set
      {
        pathToFile = value;
        if (hotkey != null &&
             !string.IsNullOrEmpty(pathToFile))
        {
          OnHotKeyUpdate();
        }
      }
    }
    #endregion

    #region events

    /// <summary>
    /// Hotkey was added / updated
    /// </summary>
    public event Action<HotKeyRow> HotKeyUpdated;

    private void OnHotKeyUpdate()
    {
      if (HotKeyUpdated != null)
      {
        HotKeyUpdated(this);
      }
    }

    /// <summary>
    /// Hotkey was deleted
    /// </summary>
    public event Action<HotKeyRow> HotKeyDeleted;

    private void OnHotKeyDeleted()
    {
      if (HotKeyDeleted != null)
      {
        HotKeyDeleted(this);
      }
    }

    #endregion

    #region constructor
    /// <summary>
    /// base constructor
    /// </summary>
    public HotKeyRow()
    {
      InitializeComponent();
      BoundEventsHandlers();

      //create id for this row / hotkey
      this.Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// constructor for saved hotkeys
    /// </summary>
    /// <param name="hotkey"></param>
    /// <param name="path"></param>
    public HotKeyRow(string id, List<Keys> hotkey, string path)
      : this()
    {
      this.id = id;
      this.hotkey = hotkey;
      this.pathToFile = path;

      this.tbHotKey.Text = Library.GetTextRepresentationOfKeyStroke(hotkey);
      this.tbPathToFile.Text = path;
    }

    //adds handlers
    private void BoundEventsHandlers()
    {
      tbHotKey.MouseDoubleClick += tbHotKey_MouseDoubleClick;
      tbPathToFile.TextChanged += tbPathToFile_TextChanged;
      btSelectFile.Click += btSelectFile_Click;
      btDeleteHotKey.Click += btDeleteHotKey_Click;
    }

    #endregion

    #region methods

    void btDeleteHotKey_Click(object sender, RoutedEventArgs e)
    {
      OnHotKeyDeleted();
    }

    void btSelectFile_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Filter = "executable file (*.exe)|*.exe|batch file (*.bat)|*.bat|all files (*.*)|*.*";
      ofd.Multiselect = false;

      if (ofd.ShowDialog() == true)
      {
        tbPathToFile.Text = ofd.FileName;
      }
    }

    void tbPathToFile_TextChanged(object sender, TextChangedEventArgs e)
    {
      PathToFile = tbPathToFile.Text;
    }

    void tbHotKey_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      InsertHotKeyDialog dialog = new InsertHotKeyDialog();
      if (dialog.ShowDialog() == true)
      {
        tbHotKey.Text = dialog.PressedKeysString;
        Hotkey = dialog.PressedKeys;
      }
    }

    List<Keys> ConvertKeysToWinAPICompatibile(List<Key> keys)
    {
      List<Keys> winAPICompatibileKeys = new List<Keys>();

      //converts keys
      foreach (Key key in keys)
      {
        winAPICompatibileKeys.Add((Keys)KeyInterop.VirtualKeyFromKey(key));
      }

      return winAPICompatibileKeys;
    }

    #endregion
  }
}
