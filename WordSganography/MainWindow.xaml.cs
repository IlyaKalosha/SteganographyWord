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
        public static string encodedFilePathStr { get; set; }
        public static string CopyPathStr { get; set; }
        public static int ContainerSize { get; set; }
        public static int MessageSize { get; set; }
        public static int BitHashSize { get; set; }
        public static string Hash { get; set; }
        public static BitArray BitHash { get; set; }
        public static BitArray BitMessage { get; set; }
        public static string MessageAndHash { get; set; }
        public static BitArray BitMessageAndHash { get; set; }


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

                MessageAndHash = messageField.Text + " " + Hash;
                BitMessageAndHash = helper.MessageToByteArray(MessageAndHash);


                //foreach(bool item in BitMessage)
                //{
                //    if (item)
                //        statusField.Text += "1";
                //    else
                //        statusField.Text += "0";
                //}
                if (ContainerSize >= BitMessageAndHash.Count)
                {
                    helper.InsertMessageToFile(FilePathStr, BitMessageAndHash);
                }
                else
                {
                    statusField.Text += "В контейнере не достаточно места \n";
                }
            }
            catch
            {
                statusField.Text += "Hash вычислен с ошибкой \n";
            }
            finally
            {
                statusField.Text += "\n Конец осаждения сообщения \n";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog FileDialog = new OpenFileDialog();
                if (FileDialog.ShowDialog() == true)
                    encodedFilePathStr = FileDialog.FileName;
                outputFilePath.Text = encodedFilePathStr;
                WordAsXML helper = new WordAsXML();
                
            }
            catch
            {

            }
            finally
            {

            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                WordAsXML helper = new WordAsXML();
                string result = helper.GetMessageFromFile(encodedFilePathStr);
                statusField.Text = result;

            }
            catch
            {

            }
            finally
            {

            }


        }
    }
}
