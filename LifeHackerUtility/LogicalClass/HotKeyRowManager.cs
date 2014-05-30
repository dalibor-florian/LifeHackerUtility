using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LifeHackerUtility.Components;
using LifeHackerUtility.HotKeyWinApiWrapper;

namespace LifeHackerUtility.LogicalClass
{
  class HotKeyRowManager
  {
    #region fields
    MainWindow window;
    Grid grid;
    Button btnAddRow;
    StorageManager storageManager;
    #endregion

    #region events

    /// <summary>
    /// event of removing hotkey
    /// </summary>
    public event Action<string> HotKeyRemove;

    /// <summary>
    /// Raise HotKeyRemove event.
    /// </summary>
    /// <param name="keys"></param>
    private void OnHotKeyRemove(string id)
    {
      if (string.IsNullOrEmpty(id)) return;
      if (HotKeyRemove != null)
      {
        HotKeyRemove(id);
      }
    }

    /// <summary>
    ///event of hotkey added
    /// </summary>
    public event Action<string, List<Keys>, string> HotKeyAdded;

    /// <summary>
    /// Raise event HotKeyAdded
    /// </summary>
    /// <param name="keys">list of pressed keys</param>
    /// <param name="pathToFile">path to runnable file</param>
    private void OnHotKeyUpdated(string id, List<Keys> keys, string pathToFile)
    {
      if (keys == null || string.IsNullOrEmpty(pathToFile)) return;
      if (HotKeyAdded != null)
      {
        HotKeyAdded(id, keys, pathToFile);
      }
    }

    #endregion

    #region constructors
    //basic constructor
    public HotKeyRowManager(MainWindow window, Grid grid, Button addRowButton, StorageManager storageManager)
    {
      this.window = window;
      this.grid = grid;
      this.btnAddRow = addRowButton;
      this.storageManager = storageManager;
      AdditionalInit();
      BoundEventHandlers();
    }

    private void AdditionalInit()
    {
      foreach (HotKeyCarryClass hotkey in storageManager.LoadHotKeys())
      {
        //merge and convert modifier keys with keys
        List<Keys> keys = MergeKeys(hotkey.ModifiersKeys, hotkey.Keys);
        //restore saved hotkeys
        AddRow(hotkey.Id, keys, hotkey.PathToFile);
      }
    }

    private void BoundEventHandlers()
    {

    }

    #endregion

    #region methods

    /// <summary>
    /// Adds new row into window
    /// </summary>
    public void AddRow()
    {
      AddRow(null, null, null);
    }
    /// <summary>
    /// Adds new row into window, where
    /// this row has already set hotkey.
    /// </summary>
    /// <param name="hotkey"></param>
    /// <param name="path"></param>
    public void AddRow(string id, List<Keys> hotkey, string path)
    {
      //creates new row
      HotKeyRow newRow =
        (id == null && hotkey == null && path == null)
        ? new HotKeyRow()
        : new HotKeyRow(id, hotkey, path);

      //bound handlers to row
      newRow.HotKeyUpdated += HotKeyUpdated;
      newRow.HotKeyDeleted += HotKeyDeleted;

      //adds to grid
      int rowNumber = AddRowToGrid();
      Grid.SetRow(newRow, rowNumber);
      Grid.SetColumn(newRow, 0);
      Grid.SetColumnSpan(newRow, 4);
      grid.Children.Add(newRow);
    }

    /// <summary>
    /// Handler of HotKeyUpdated event
    /// </summary>
    /// <param name="row"></param>
    void HotKeyUpdated(HotKeyRow row)
    {
      OnHotKeyUpdated(row.Id, row.Hotkey, row.PathToFile);
    }
    /// <summary>
    /// Handler of button click, wich remove specified hotkey.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HotKeyDeleted(HotKeyRow row)
    {
      OnHotKeyRemove(row.Id);
      int rowIndex = Grid.GetRow(row);
      RemoveRowFromGrid(rowIndex);
    }

    /// <summary>
    /// Adds new row to grid control
    /// </summary>
    /// <returns>Returns index of new row.</returns>
    private int AddRowToGrid()
    {
      //how much rows is already in grid
      int rowCount = grid.RowDefinitions.Count;

      //add row definition
      grid.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto //auto height
      });

      //move content of footer to last row
      Grid.SetRow(btnAddRow, rowCount);

      //returns number of new row
      return rowCount - 1;
    }
    /// <summary>
    /// Remove row with specified index.
    /// </summary>
    /// <param name="index">Index of row which will be remove.</param>
    private void RemoveRowFromGrid(int index)
    {
      #region remove elements on <index>. row
      for (int i = 0; i < grid.Children.Count; i++)
      {
        UIElement child = grid.Children[i];

        //element is on specified row
        if (index == Grid.GetRow(child))
        {
          //removes element
          grid.Children.Remove(child);
          //decrement variable to avoid 
          //jump over some element in grid
          i--;
        }
      }
      #endregion

      #region move element up
      foreach (UIElement child in grid.Children)
      {
        int gridRow = Grid.GetRow(child);
        //moves elements up to fill emptied row
        if (gridRow > index)
        {
          Grid.SetRow(child, gridRow - 1);
        }
      }
      #endregion

      #region remove last row
      int lastRowIndex = grid.RowDefinitions.Count - 1;
      grid.RowDefinitions.RemoveAt(lastRowIndex);
      #endregion
    }

    /// <summary>
    /// Adds converted modifier keys into keys list
    /// </summary>
    /// <param name="mkeys"></param>
    /// <param name="keys"></param>
    private List<Keys> MergeKeys(List<ModifierKeys> mkeys, List<Keys> keys)
    {
      List<Keys> allKeys = new List<Keys>();

      //converts modifier keys and adds them to list
      foreach (ModifierKeys mkey in mkeys)
      {
        switch (mkey)
        {
          case ModifierKeys.Alt:
            allKeys.Add(Keys.Alt);
            break;
          case ModifierKeys.Control:
            allKeys.Add(Keys.Control);
            break;
          case ModifierKeys.Shift:
            allKeys.Add(Keys.Shift);
            break;
          case ModifierKeys.Windows:
            allKeys.Add(Keys.LWin);
            break;
        }
      }

      //adds keys
      allKeys.AddRange(keys);

      return allKeys;
    }

    #endregion
  }
}
