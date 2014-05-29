using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LifeHackerUtility.HotKeyWinApiWrapper;

namespace LifeHackerUtility.LogicalClass
{
  class Library
  {
    /// <summary>
    /// Returns human readable representation of keystroke.
    /// </summary>
    /// <param name="keystroke"></param>
    /// <returns></returns>
    public static string GetTextRepresentationOfKeyStroke(List<Keys> keystroke)
    {
      //there is no keys in keystroke
      if (keystroke.Count == 0) return string.Empty;

      string separator = " + ";

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < keystroke.Count; i++)
      {
        sb.Append(keystroke[i].ToString());
        if (i != keystroke.Count - 1) sb.Append(separator);
      }

      return sb.ToString();
    }
  }
}
