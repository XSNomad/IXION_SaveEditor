using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IXION_SaveEditor.core;
using IXION_SaveEditor.MVVM.view;
using IXION_SaveEditor.MVVM.viewmodel;
using Microsoft.Win32;

namespace IXION_SaveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            LoadAndModifySave(true);
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            CheatsSP.Visibility = Visibility.Hidden;
            SettingsSP.Visibility = Visibility.Visible;
        }
        private void Cheats_Click(object sender, RoutedEventArgs e)
        {
            SettingsSP.Visibility = Visibility.Hidden;
            CheatsSP.Visibility = Visibility.Visible;
        }
        private void Stability_Click(object sender, RoutedEventArgs e)
        {
            SettingsSP.Visibility = Visibility.Visible;
        }
        private void Citizens_Click(object sender, RoutedEventArgs e)
        {
            SettingsSP.Visibility = Visibility.Visible;
        }
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadAndModifySave(false);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAndModifySave(false);
        }
        private const string playerSave = "\\Player.sav";
        internal void LoadAndModifySave(bool modify)
        {
            string PlayerSaveFile = Find_LastSave() + playerSave;
            if (File.Exists(PlayerSaveFile))
            {
                //saveHeader.Text += $" - {PlayerSaveFile}";
                var lines = File.ReadLines(PlayerSaveFile).ToList();
                for (int i = 0; i < 339; i++)
                {
                    ParseLine(lines, i, 3, spSettings1, modify);
                    ParseLine(lines, i, 3, spSettings2, modify);
                    ParseLine(lines, i, 3, spSettings3, modify);
                    ParseLine(lines, i, 3, spSettings4, modify);
                    ParseLine(lines, i, 3, spSettings5, modify);
                    ParseLine(lines, i, 0, spCheats1, modify);
                    ParseLine(lines, i, 0, spCheats2, modify);
                }
                if (modify)
                {
                    File.WriteAllLines(PlayerSaveFile, lines);
                }
            }
        }
        internal void ParseLine(List<string> lines, int i, int peek, StackPanel sp, bool apply)
        {
            var tbItem = sp.Children.OfType<TextBox>();
            var cbItem = sp.Children.OfType<CheckBox>();
            foreach (TextBox textBox in tbItem)
            {
                if (lines[i].Trim().StartsWith($"\"{textBox.Name}"))
                {
                    var split = lines[i + peek].Split(':');
                    var digit = new string((from c in split[1]
                                            where char.IsDigit(c)
                                            select c).ToArray());
                    if (!apply)
                    {
                        textBox.Text = digit;
                    }
                    else
                    {
                        lines[i + peek] = lines[i + peek].Replace(digit, textBox.Text);
                    }
                }
            }
            foreach (CheckBox checkBox in cbItem)
            {
                var name = checkBox.Content.ToString();
                var tmp2 = lines[i + peek];
                if (lines[i].Contains(name))
                {
                    var split = lines[i + peek].Split(':');
                    string enabled = new((from c in split[1]
                                          where char.IsLetter(c)
                                          select c).ToArray());
                    if (!apply)
                    {
                        if (enabled == "true") checkBox.IsChecked = true;
                    }
                    else
                    {
                        var checkBoxEnabled = checkBox.IsChecked.ToString().ToLower();
                        lines[i + peek] = lines[i + peek].Replace(enabled, checkBoxEnabled);
                    }
                }
            }
        }
        internal string Find_LastSave()
        {
            DirectoryInfo diTop = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + "\\BulwarkStudios\\Ixion\\Saves\\");
            DateTime lastSave = new(2018, 6, 25, 10, 21, 23);
            string lastSavePath = "";
            try
            {
                foreach (var di in diTop.EnumerateDirectories("*"))
            {

                    if (di.Name.Contains('-')) continue;
                    if (lastSave < di.CreationTime)
                    {
                        lastSave = di.CreationTime;
                        lastSavePath = di.FullName;
                    }
                }
            }
            catch (Exception ex)
            {
                string messageBoxText = $"Error Detecting Save: {ex}. Directory: {diTop}";
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }

            if (lastSavePath == "")
            {
                var dlg = new FolderPicker
                {
                    InputPath = @"c:\"
                };
                if (dlg.ShowDialog() == true)
                {
                    lastSavePath = dlg.ResultPath;
                }
            }
            return lastSavePath;
        }
    }
}
