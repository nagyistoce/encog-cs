using System;
using System.IO;
using System.Text;
using Encog.Bot;

namespace Encog.Util.File
{
    /// <summary>
    /// Contains several utilities for working with files.
    /// </summary>
    public class FileUtil
    {
        /// <summary>
        /// Add, or replace a filename base.  A filename base is between an underbar
        /// and the . for the extension.  For example: "myfile_raw.csv", the base is
        /// "raw".
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static FileInfo AddFilenameBase(FileInfo filename, String bs)
        {
            String f = GetFileName(filename);
            String ext = GetFileExt(filename);

            int idx1 = f.LastIndexOf('_');
            int idx2 = f.LastIndexOf(Path.PathSeparator);

            bool remove = false;

            if (idx1 != -1)
            {
                if (idx2 == -1)
                {
                    remove = true;
                }
                else
                {
                    remove = idx1 > idx2;
                }
            }

            if (remove)
            {
                f = f.Substring(0, (idx1) - (0));
            }

            return new FileInfo(f + bs + "." + ext);
        }

        /// <summary>
        /// Get the filename, without extension.
        /// </summary>
        /// <param name="file">The file to parse.</param>
        /// <returns>The file name.</returns>
        public static String GetFileName(FileInfo file)
        {
            String fileName = file.ToString();
            int mid = fileName.LastIndexOf(".");
            if (mid == -1)
            {
                return fileName;
            }
            return fileName.Substring(0, (mid) - (0));
        }

        /// <summary>
        /// Get the file extension.
        /// </summary>
        /// <param name="file">The base file.</param>
        /// <returns>The extension.</returns>
        public static String GetFileExt(FileInfo file)
        {
            String fileName = file.ToString();
            int mid = fileName.LastIndexOf(".");
            if (mid == -1)
                return "";
            return fileName.Substring(mid + 1, (fileName.Length) - (mid + 1));
        }

        /// <summary>
        /// Read a file into a string.
        /// </summary>
        /// <param name="filePath">The file to read.</param>
        /// <returns>The contents of the file.</returns>
        public static String ReadFileAsString(FileInfo filePath)
        {
            var fileData = new StringBuilder(1000);
            TextReader reader = new StreamReader(filePath.OpenRead());
            var buf = new char[1024];
            int numRead = 0;
            while ((numRead = reader.Read(buf, 0, buf.Length)) != -1)
            {
                var readData = new string(buf, 0, numRead);
                fileData.Append(readData);
                buf = new char[1024];
            }
            reader.Close();
            return fileData.ToString();
        }

        /// <summary>
        /// Change a file's extension.
        /// </summary>
        /// <param name="name">The filename to change.</param>
        /// <param name="ext">The new extension.</param>
        /// <returns></returns>
        public static String ForceExtension(String name, String ext)
        {
            String b = GetFileName(new FileInfo(name));
            return b + "." + ext;
        }

        public static void WriteFileAsString(FileInfo path, String str)
        {
            FileStream fs = path.OpenRead();
            var writer = new StreamWriter(fs);
            writer.Write(str);
            writer.Close();
            fs.Close();
        }

        public static void Copy(FileInfo source, FileInfo target)
        {
            try
            {
				target.Delete();
                FileStream fos = target.OpenWrite();
                Stream mask0 = source.OpenRead();

                Copy(mask0, fos);

                fos.Close();
                mask0.Close();
            }
            catch (IOException e)
            {
                throw new EncogError(e);
            }
        }

        public static void Copy(Stream mask0, Stream os)
        {
            try
            {
                var buffer = new byte[BotUtil.BUFFER_SIZE];
                int length;
                do
                {
                    length = mask0.Read(buffer, 0, buffer.Length);

                    if (length >= 0)
                    {
                        os.Write(buffer, 0, length);
                    }
                } while (length >= 0);
            }
            catch (IOException ex)
            {
                throw new EncogError(ex);
            }
        }

        public static void CopyResource(String resource, FileInfo targetFile)
        {
            try
            {
                Stream mask0 = ResourceInputStream
                    .OpenResourceInputStream(resource);
				targetFile.Delete();
                Stream os = targetFile.OpenWrite();
                Copy(mask0, os);
                mask0.Close();
                os.Close();
            }
            catch (IOException ex)
            {
                throw new EncogError(ex);
            }
        }

        public static FileInfo CombinePath(FileInfo dir, string f)
        {
            string s = dir.ToString();
            return new FileInfo(Path.Combine(s, f));
        }
    }
}