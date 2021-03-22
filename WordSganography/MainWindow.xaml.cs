using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Security.Cryptography;
using System.Collections;

namespace WordSganography
{

    public partial class MainWindow : Window
    {
        public static string FilePathStr { get; set; }
        public static string CopyPathStr { get; set; }
        public static int ContainerSize { get; set; }
        public static int MessageSize { get; set; }
        public static int BitHashSize { get; set; }
        public static string Hash { get; set; }
        public static BitArray BitHash { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog FileDialog = new OpenFileDialog();
                if (FileDialog.ShowDialog() == true)
                    FilePathStr = FileDialog.FileName;
                    FilePath.Text = FilePathStr;
                WordAsXML helper = new WordAsXML();
                ContainerSize = helper.ContainerSize(FilePathStr);
                containerSizeField.Text = ContainerSize.ToString();
            }
            catch
            {

            }
            finally
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                statusField.Text = "";

                WordAsXML helper = new WordAsXML();

                Hash = helper.HashCounter(messageField.Text);
                hashField.Text = Hash;
                BitHash = helper.MessageToByteArray(Hash);
                BitHashSize = BitHash.Length;

                BitArray res = helper.MessageToByteArray(messageField.Text);
                MessageSize = res.Length;

                if (ContainerSize >= BitHashSize + MessageSize)
                {
                    helper.InsertMessageToFile(FilePathStr);
                }
                else
                {
                    statusField.Text = "В контейнере не достаточно места \n";
                }
            }
            catch
            {
                statusField.Text = "Hash вычислен с ошибкой \n";
            }
            finally
            {
                statusField.Text += "Конец осаждения сообщения \n";
            }
        }
    }
}
