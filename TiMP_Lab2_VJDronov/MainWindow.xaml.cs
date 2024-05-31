using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TiMP_Lab2_VJDronov
{
    public partial class MainWindow : Window
    {
        private readonly string textRemained = "Осталось до окончания пробной версии: ";
        private readonly string fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\names.txt";
        private readonly string regPath = "Software\\ImportantSecurity";
        private readonly long defaultTime = 2 * 60;
        private readonly int defaultStarts = 5;
        private readonly byte defaultMode = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxName.Text))
            {
                MessageBox.Show("Пожалуйста, введите ФИО", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                byte[] tmpSource = Encoding.UTF8.GetBytes(TextBoxName.Text);
                byte[] tmpHash = new SHA256CryptoServiceProvider().ComputeHash(tmpSource);

                if (BitConverter.ToUInt32(tmpHash) == 3251860271)
                {
                    MainGrid.Visibility = Visibility.Hidden;
                    AdminGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    string inputText = TextBoxName.Text;
                    if (File.Exists(fileName))
                    {
                        string[] fileText = File.ReadAllLines(fileName, Encoding.UTF8);
                        foreach (string line in fileText)
                        {
                            if (inputText.Equals(line))
                            {
                                MessageBox.Show("Данное ФИО уже имеется в файле", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                        }
                    }
                    
                    RegistryKey myKey = Registry.CurrentUser;
                    RegistryKey signatureFolder = myKey.CreateSubKey(regPath, true);
                    object startsLeft = signatureFolder.GetValue("StartsLeft");
                    object expireTime = signatureFolder.GetValue("ExpireTime");
                    object returnedMode = signatureFolder.GetValue("Mode");
                    int mode = (returnedMode == null) ? defaultMode : (int)returnedMode;
                    signatureFolder.SetValue("Mode", mode);
                    switch (mode)
                    {
                        case 0:
                            if (startsLeft == null)
                            {
                                signatureFolder.SetValue("StartsLeft", defaultStarts - 1);
                                using (StreamWriter sw = File.AppendText(fileName)) { sw.WriteLine(inputText); }
                                MessageBox.Show("Оставшееся количество использований: " + (defaultStarts - 1), "Пробная версия", MessageBoxButton.OK, MessageBoxImage.Information);
                                TextBoxName.Text = "";
                                TextLimit.Text = textRemained + (defaultStarts - 1) + " использований";
                            }
                            else if ((int)startsLeft <= 0)
                            {
                                MessageBox.Show("Пробная версия ПО истекла. Пожалуйста, приобретите полную версию", "Приобретение полной версии", MessageBoxButton.OK, MessageBoxImage.Information);
                                TextLimit.Text = textRemained + "0 использований. Пожалуйста, приобретите полную версию";
                            }
                            else
                            {
                                using (StreamWriter sw = File.AppendText(fileName)) { sw.WriteLine(inputText); }
                                int newStartsLeft = (int)startsLeft - 1;
                                signatureFolder.SetValue("StartsLeft", newStartsLeft);
                                MessageBox.Show("Оставшееся количество использований: " + newStartsLeft, "Пробная версия", MessageBoxButton.OK, MessageBoxImage.Information);
                                TextBoxName.Text = "";
                                TextLimit.Text = textRemained + newStartsLeft + " использований";
                            }
                            signatureFolder.Close();
                            return;
                        case 1:
                            long currentTime = (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                            if (expireTime == null)
                            {
                                signatureFolder.SetValue("ExpireTime", currentTime + defaultTime, RegistryValueKind.QWord);
                                using (StreamWriter sw = File.AppendText(fileName)) { sw.WriteLine(inputText); }
                                MessageBox.Show("Оставшееся время использования: " + defaultTime + " секунд", "Пробная версия", MessageBoxButton.OK, MessageBoxImage.Information);
                                TextBoxName.Text = "";
                                TextLimit.Text = textRemained + defaultTime + " секунд";
                            }
                            else if (currentTime >= (long)expireTime)
                            {
                                MessageBox.Show("Пробная версия ПО истекла. Пожалуйста, приобретите полную версию", "Приобретение полной версии", MessageBoxButton.OK, MessageBoxImage.Information);
                                TextLimit.Text = textRemained + "0 секунд. Пожалуйста, приобретите полную версию";
                            }
                            else
                            {
                                using (StreamWriter sw = File.AppendText(fileName)) { sw.WriteLine(inputText); }
                                MessageBox.Show("Оставшееся время использования: " + ((long)expireTime - currentTime) + " секунд", "Пробная версия", MessageBoxButton.OK, MessageBoxImage.Information);
                                TextBoxName.Text = "";
                                TextLimit.Text = textRemained + ((long)expireTime - currentTime) + " секунд";
                            }
                            signatureFolder.Close();
                            return;
                    }
                }
            }
        }

        private void TextBoxName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            { 
                BtnSubmit_Click(sender, e);
            }
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            AdminGrid.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Visible;
        }

        private void RadioBtnStart_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxTime.Visibility = Visibility.Hidden;
            TextBoxTime.Text = "";
            TextBoxStart.Visibility = Visibility.Visible;
        }

        private void RadioBtnTime_Checked(object sender, RoutedEventArgs e)
        {
            TextBoxStart.Visibility = Visibility.Hidden;
            TextBoxStart.Text = "";
            TextBoxTime.Visibility = Visibility.Visible;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (RadioBtnStart.IsChecked == true)
            {
                RegistryKey myKey = Registry.CurrentUser;
                RegistryKey signatureFolder = myKey.CreateSubKey(regPath, true);
                signatureFolder.SetValue("Mode", 0);
                signatureFolder.SetValue("StartsLeft", int.Parse(TextBoxStart.Text));
                MessageBox.Show("Установлено количество использований: " + TextBoxStart.Text, "Меню администратора", MessageBoxButton.OK, MessageBoxImage.Information);
                TextLimit.Text = textRemained + TextBoxStart.Text + " использований";
                signatureFolder.Close();
            }

            else if (RadioBtnTime.IsChecked == true)
            {
                RegistryKey myKey = Registry.CurrentUser;
                RegistryKey signatureFolder = myKey.CreateSubKey(regPath, true);
                long newTime = long.Parse(TextBoxTime.Text);
                long currentTime = (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                signatureFolder.SetValue("Mode", 1);
                signatureFolder.SetValue("ExpireTime", currentTime + newTime, RegistryValueKind.QWord);
                MessageBox.Show("Установлено время доступа: " + TextBoxTime.Text + " секунд", "Меню администратора", MessageBoxButton.OK, MessageBoxImage.Information);
                TextLimit.Text = textRemained + newTime + " секунд";
                signatureFolder.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RadioBtnStart.IsChecked = true;
            RegistryKey myKey = Registry.CurrentUser;
            RegistryKey signatureFolder = myKey.CreateSubKey(regPath, true);
            object returnedMode = signatureFolder.GetValue("Mode");
            object startsLeft = signatureFolder.GetValue("StartsLeft");
            object expireTime = signatureFolder.GetValue("ExpireTime");
            if (returnedMode != null)
            {
                switch ((int)returnedMode)
                {
                    case 0:
                        if (startsLeft != null)
                        {
                            TextLimit.Text = textRemained + (int)startsLeft + " использований";
                        }
                        break;
                    case 1:
                        if (expireTime != null)
                        {
                            long currentTime = (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                            bool isExpired = currentTime > (long)expireTime;
                            TextLimit.Text = textRemained + (isExpired ? 0 : (long)expireTime - currentTime) + " секунд" + (isExpired ? ". Пожалуйста, приобретите полную версию" : "");
                        }
                        break;
                }
            }
            else if (startsLeft == null && expireTime == null)
            {
                TextLimit.Text = "";
            }

            signatureFolder.Close();
        }
    }
}
