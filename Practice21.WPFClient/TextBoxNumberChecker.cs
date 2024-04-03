using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Practice21.WPFClient
{
    static class TextBoxNumberChecker
    {
        public static void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string txt = (string)e.DataObject.GetData(typeof(string));
                if (!Regex.IsMatch(txt, @"[\s()\-+0-9]"))
                {
                    e.CancelCommand();
                }
                int start = ((TextBox)sender).SelectionStart;
                int length = ((TextBox)sender).SelectionLength;
                int caret = ((TextBox)sender).CaretIndex;
                string result = Regex.Replace(txt, @"[^\s()\-+0-9]", "");
                string text = ((TextBox)sender).Text.Substring(0, start);
                text += ((TextBox)sender).Text.Substring(start + length);

                string newText = text.Substring(0, ((TextBox)sender).CaretIndex) + result;
                newText += text.Substring(caret);
                ((TextBox)sender).Text = newText;
                ((TextBox)sender).CaretIndex = caret + result.Length;
                e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        public static void txtPhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[^0-9-+()]+");
        }

    }
}
