using System;
using System.Text.RegularExpressions;

namespace BViewer
{
    public static class Utils
    {
        public static Regex fileRegex = new Regex("\\.(png|bmp|jpg|jpeg|gif|tif|tiff|tga)");

        public static bool IsFileSupported(string path)
        {
            return fileRegex.IsMatch(path);
        }

        public static bool ComparePaths(string path1, string path2)
        {
            try
            {
                return string.Compare(System.IO.Path.GetFullPath(path1), System.IO.Path.GetFullPath(path2), true) == 0;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }
    }
}
