using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LifeHackerUtility.LogicalClass;
using LifeHackerUtility.HotKeyWinApiWrapper;

namespace LifeHackerUtility
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    #region fields
    HotKeyRowManager rowManager;
    HotKeyManager hotkeyManager;
    StorageManager storageManager;
    #endregion

    #region constructors
    /// <summary>
    /// main constuctor
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
      AdditionalInit();
      BoundEventHandlers();
    }

    //does others inits
    private void AdditionalInit()
    {
      storageManager = new StorageManager();
      rowManager = new HotKeyRowManager(this, grMain, btnAddHotKey, storageManager);
      hotkeyManager = new HotKeyManager(this, storageManager);
    }

    //add handlers to controls
    private void BoundEventHandlers()
    {
      this.Loaded += WindowLoaded;
      this.btnAddHotKey.Click += Click_AddHotKey;
      this.rowManager.HotKeyAdded += rowManager_HotKeyAdded;
      this.rowManager.HotKeyRemove += rowManager_HotKeyRemove;
    }



    #endregion

    #region methods

    #region handlers

    //handle first show of window
    void WindowLoaded(object sender, RoutedEventArgs e)
    {
      //this.WindowState = System.Windows.WindowState.Minimized;

      //this.trayIcon.Icon = System.Drawing.Icon.FromHandle(Properties.Resources.icon_large.GetHicon());
      //this.trayIcon.ShowBalloonTip(Constants.TRAY_BALOON_TIP_TIME, "Aplikace běží", "Aplikace LifeHacker běží v tray ikoně.", ToolTipIcon.None);
    }

    //click on button add hot key
    private void Click_AddHotKey(object sender, RoutedEventArgs e)
    {
      rowManager.AddRow();
    }

    //react on hotkey remove event
    void rowManager_HotKeyRemove(string id)
    {
      hotkeyManager.RemoveHotKey(id);
    }

    //react on hotkey added event
    void rowManager_HotKeyAdded(string id, List<Keys> keys, string pathToFile)
    {
      hotkeyManager.AddHotKey(id, keys, pathToFile);
    }

    #endregion

    #endregion

  }
}
