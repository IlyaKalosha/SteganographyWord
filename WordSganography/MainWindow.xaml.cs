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
        public static bool isFileDoc { get; set; }

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
                //open file from a directory
                OpenFileDialog FileDialog = new OpenFileDialog();
                FileDialog.Filter = "Word File (.docx ,.doc)|*.docx;*.doc";
                if (FileDialog.ShowDialog() == true)
                    FilePathStr = FileDialog.FileName;
                    FilePath.Text = FilePathStr;

                //create a work copy and validate .doc/.docx
                WordAsXML helper = new WordAsXML();
                FilePathStr = helper.CreateDocCopy(FilePathStr);
                isFileDoc = helper.ValidateFile(FilePathStr);
                if (isFileDoc)
                {
                    FilePathStr = helper.ConvertDocToDocx(FilePathStr);
                }
                //count file spaces and print the result
                ContainerSize = helper.ContainerSize(FilePathStr);
                containerSizeField.Text = ContainerSize.ToString();
            }
            catch (Exception ex)
            {
                statusField.Text +=ex.Message + "\n";
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
                if (ContainerSize >= BitMessageAndHash.Count)
                {
                    helper.InsertMessageToFile(FilePathStr, BitMessageAndHash);
                    if (isFileDoc)
                    {
                        helper.ConvertDocxToDoc(FilePathStr);
                        isFileDoc = false;
                    }
                    statusField.Text = "Сообщение осаждено\n";
                }
                else
                {
                    throw new Exception("В контейнере не достаточно места");
                }
            }
            catch (Exception ex)
            {
                statusField.Text +=ex.Message + "\n";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog FileDialog = new OpenFileDialog();
                FileDialog.Filter = "Word File (.docx ,.doc)|*.docx;*.doc";
                if (FileDialog.ShowDialog() == true)
                    encodedFilePathStr = FileDialog.FileName;
                outputFilePath.Text = encodedFilePathStr;
            }
            catch (Exception ex)
            {
                statusField.Text +=ex.Message + "\n";
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                WordAsXML helper = new WordAsXML();
                string result = helper.GetMessageFromFile(encodedFilePathStr);
                if (result != null)
                {
                    var lastSpacePos = result.LastIndexOf(' ');
                    string message = result.Substring(0, lastSpacePos);
                    string hash = result.Substring(lastSpacePos + 1, result.Length - lastSpacePos - 1);
                    outputMessageField.Text = message;
                    outputHashField.Text = hash;
                    controlHashField.Text = helper.HashCounter(message);
                    statusField.Text += "Сообщение извлечено" + "\n";
                }
                else
                {
                    throw new Exception("Сообщение не было извлечено");
                }
            }
            catch(Exception ex)
            {
                statusField.Text +=ex.Message + "\n";   
            }
        }
    }
}
