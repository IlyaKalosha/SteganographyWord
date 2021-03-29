using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Cryptography;
using System.Collections;
using DocumentFormat.OpenXml;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace WordSganography
{
    public class WordAsXML
    {
        public WordAsXML()
        {
        }

        public string CreateDocCopy(string FilePathStr)
        {
            string sourceFile = FilePathStr;
            string destinationFile;
            if (FilePathStr.ToLower().EndsWith(".doc"))
            {
                 destinationFile = FilePathStr.Replace(".doc", "v2.doc");
            }
            else
            {
                 destinationFile = FilePathStr.Replace(".docx", "v2.docx");
            }
            try
            {
                File.Copy(sourceFile, destinationFile, true);
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
            return destinationFile;
        }

        public bool ValidateFile(string FilePathStr)
        {
            if (FilePathStr.ToLower().EndsWith(".doc"))
                return true;
            else
                return false;
        }

        public string ConvertDocxToDoc(string FilePathStr)
        {
            Application word = new Application();
            string newFileName = null;
            if (FilePathStr.ToLower().EndsWith(".docx"))
            {
                var sourceFile = new FileInfo(FilePathStr);
                try
                {
                    var document = word.Documents.Open(sourceFile.FullName);

                    newFileName = sourceFile.FullName.Replace(".docx", ".doc");
                    document.SaveAs2(newFileName, WdSaveFormat.wdFormatDocumentDefault,
                                     CompatibilityMode: WdCompatibilityMode.wdCurrent);

                }
                catch
                {
                    
                }
                finally
                {
                    word.ActiveDocument.Close();
                    word.Quit();
                    File.Delete(FilePathStr);
                }
                return newFileName;
            }
            else
            {
                throw new Exception("Convert .docx to .doc failed");
            }
        }

        public string ConvertDocToDocx(string FilePathStr)
        {
            Application word = new Application();
            string newFileName = null;
            if (FilePathStr.ToLower().EndsWith(".doc"))
            {
                var sourceFile = new FileInfo(FilePathStr);
                try
                {
                    var document = word.Documents.Open(sourceFile.FullName);

                    newFileName = sourceFile.FullName.Replace(".doc", ".docx");
                    document.SaveAs2(newFileName, WdSaveFormat.wdFormatXMLDocument, ReadOnlyRecommended: false,
                                     CompatibilityMode: WdCompatibilityMode.wdWord2013);
                }
                catch
                {

                }
                finally
                {
                    word.ActiveDocument.Close();
                    word.Quit();
                }
                File.Delete(FilePathStr);

                return newFileName;
            }
            else
            {
                throw new Exception("Convert .doc to .docx failed");
            }
        }

        public string InsertMessageToFile(string FilePathStr, BitArray message, bool isHashNeed)
        {
            int counter = 0;

            using (WordprocessingDocument doc = WordprocessingDocument.Open(FilePathStr, true))
            {
                Body mainPart = doc.MainDocumentPart.Document.Body;
                var runs = mainPart.Descendants<Run>().ToList();

                foreach (Run run in runs)
                {
                    var text = run.GetFirstChild<Text>();
                    if (text != null)
                    {
                        if (text.Text != null & text.Text != " ")
                        {
                            string[] words = text.Text.Split(' ');
                            for (int i = 0; i < words.Count(); i++)
                            {
                                string word = words[i];
                                var newRun = (Run)run.Clone();
                                string newWord = word;
                                Text newRunText = newRun.GetFirstChild<Text>();
                                newRunText.Text = newWord;
                                run.Parent.InsertBefore(newRun, run);

                                Run newSpaceRun = new Run();
                                string space = (i < words.Count() - 1 ? " " : "");
                                RunProperties runProperties = new RunProperties();
                                if (space == " ")
                                {
                                    if (counter < message.Count)
                                    {
                                        if (message.Get(counter))
                                        {
                                            runProperties.AppendChild(new Position() { Val = "2" });
                                            counter++;
                                        }
                                        else
                                        {
                                            runProperties.AppendChild(new Position() { Val = "-2" });
                                            counter++;
                                        }
                                    }
                                }
                                runProperties.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Languages() { Val = "ru-RU" });
                                newSpaceRun.AppendChild(runProperties);
                                Text newSpace = newSpaceRun.AppendChild(new Text(space));
                                newSpace.Space = SpaceProcessingModeValues.Preserve;
                                run.Parent.InsertBefore(newSpaceRun, run);
                            }
                            run.Remove();
                        }
                    }
                    if (counter >= message.Count)
                        break;
                }
                doc.Save();
                doc.Close();
            }
            return null;
        }

        public string GetMessageFromFile(string FilePathStr)
        {
            string result="";
            string lastResult = "";
            using (WordprocessingDocument doc = WordprocessingDocument.Open(FilePathStr, true))
            {
                Body body = doc.MainDocumentPart.Document.Body;
                foreach(Run item in body.Descendants<Run>().ToList())
                {
                    if(item.InnerText==" " & item.GetFirstChild<RunProperties>()!=null )
                        if(item.GetFirstChild<RunProperties>().GetFirstChild<Position>() != null) {
                            {
                                if (item.GetFirstChild<RunProperties>().GetFirstChild<Position>().Val == "2")
                                    result += "1";
                                else if (item.GetFirstChild<RunProperties>().GetFirstChild<Position>().Val == "-2")
                                {
                                    result += "0";
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                }
                var bitArr = new BitArray(result.Select(c => c == '1').ToArray());
                lastResult = ByteArrayToMessage(bitArr);
            }
            return lastResult;
        }

        public BitArray MessageToByteArray(string message)
        {

            byte[] bytes;
            bytes = Encoding.UTF8.GetBytes(message);
            BitArray bit = new BitArray(bytes);

            return bit;
        }

        public string ByteArrayToMessage(BitArray bites)
        {
            byte[] strArr = new byte[bites.Length / 8];

            for(int i = 0; i < bites.Length / 8; i++)
            {
                for(int index = i * 8, m = 1; index < i * 8 + 8; index++, m *= 2)
                {
                    strArr[i] += bites.Get(index) ? (byte)m : (byte)0;
                }
            }

            return Encoding.UTF8.GetString(strArr);
        }

        public int ContainerSize(string FilePathStr)
        {
            int counter = 0;
            string text = null;

            using (WordprocessingDocument doc = WordprocessingDocument.Open(FilePathStr, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                text += body.InnerText;
                foreach(var item in text)
                {
                    if (item == ' ')
                        counter++;
                }
            }
            return counter;
        }

        public string HashCounter(string message)
        {
            string result = null;

            byte[] byteMessage = Encoding.ASCII.GetBytes(message);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashMessage = md5.ComputeHash(byteMessage);
            result = hashMessage.Aggregate("", (current, next) => current + next);

            return result;
        }
    }
}
