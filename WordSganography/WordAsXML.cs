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

namespace WordSganography
{
    public class WordAsXML
    {
        public WordAsXML()
        {
        }

        public string CreateDocCopy(string FilePathStr)
        {
            string newPath = "";
            string withoutEnd = "";
            if (FilePathStr[FilePathStr.Length-1].Equals('x'))
            {
                withoutEnd += FilePathStr.Substring(0, FilePathStr.Length - 5);
                newPath += withoutEnd + "_new.docx";
            }
            if (FilePathStr[FilePathStr.Length-1].Equals('c'))
            {
                withoutEnd += FilePathStr.Substring(0, FilePathStr.Length - 4);
                newPath += withoutEnd + "_new.doc";
            }
            using (WordprocessingDocument doc = WordprocessingDocument.Open(FilePathStr, true))
            {
                doc.SaveAs(newPath);
                doc.Close();
            }
            return newPath;
        }

        public string InsertMessageToFile(string FilePathStr)
        {

            using (WordprocessingDocument doc = WordprocessingDocument.Open(FilePathStr, true))
            {
                Body mainPart = doc.MainDocumentPart.Document.Body;

                var runs = mainPart.Descendants<Run>().ToList();

                foreach (Run run in runs)
                {
                    var text = run.GetFirstChild<Text>();
                    if (text.Text != null)
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
                            string space = (i < words.Count() ? " " : "");

                            RunProperties runProperties = new RunProperties();
                            runProperties.AppendChild(new Position() { Val = "4" });
                            runProperties.AppendChild(new Languages() { Val = "ru-RU" });
                            newSpaceRun.AppendChild(runProperties);
                            Text newSpace = newSpaceRun.AppendChild(new Text(space));
                            newSpace.Space = SpaceProcessingModeValues.Preserve;

                            run.Parent.InsertBefore(newSpaceRun, run);
                        }
                        run.Remove();
                    }
                }
                doc.Save();
                doc.Close();
            }
            return null;
        }

        public string GetMessageFromFile(string FilePathStr)
        {
            return null;
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
