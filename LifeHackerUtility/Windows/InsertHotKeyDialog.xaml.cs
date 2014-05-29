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
using System.Windows.Shapes;
using LifeHackerUtility.HotKeyWinApiWrapper;

namespace LifeHackerUtility.Windows
{
  /// <summary>
  /// Interaction logic for InsertHotKeyDialog.xaml
  /// </summary>
  public partial class InsertHotKeyDialog : Window
  {
    #region fields

    KeyStrokeRecorder ks;
    List<Keys> pressedKeys;

    #endregion

    #region properties

    //retain pressed keys
    public List<Keys> PressedKeys
    {
      get { return pressedKeys; }
    }
    //string representation of pressed keys
    public string PressedKeysString
    {
      get { return ks.GetTextRepresentationOfKeyStroke(); }
    }

    #endregion

    #region constuctors
    //basic constructor
    public InsertHotKeyDialog()
    {
      InitializeComponent();
      AdditionalInit();
      BoundEventHandlers();
    }
    //other initialization
    private void AdditionalInit()
    {
      ks = new KeyStrokeRecorder(this);
      btnConfirm.Visibility = Visibility.Hidden;
    }
    //adds events handlers
    private void BoundEventHandlers()
    {
      ks.KeyStrokeCompleted += (pressedKeysList) =>
      {
        pressedKeys = pressedKeysList;
        lblHotKey.Content = ks.GetTextRepresentationOfKeyStroke();
        btnConfirm.Visibility = Visibility.Visible;
      };
      btnConfirm.Click += (sender, e) =>
      {
        this.DialogResult = true;
        this.Close();
      };
    }

    #endregion

    #region methods

    //nothing for now but
    //TODO escape closes this window without saving keystroke
    
    #endregion

  }
}
