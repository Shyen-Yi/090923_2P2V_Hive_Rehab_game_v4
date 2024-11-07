using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.IO.Compression;

namespace com.hive.projectr
{
    public class FileUtil
    {
        public static bool CreateZipFromDirectories(string[] sourceDirectories, string destinationZipFilePath, HashSet<string> excludeFileExtensions)
        {
            if (sourceDirectories == null || sourceDirectories.Length < 1)
            {
                Logger.LogError($"No source directories");
                return false;
            }

            string tempDirectoryPath = Path.Combine(Application.temporaryCachePath, "TempZip");
            Directory.CreateDirectory(tempDirectoryPath);

            try
            {
                foreach (var dirPath in sourceDirectories)
                {
                    if (Directory.Exists(dirPath))
                    {
                        var dirName = Path.GetDirectoryName($"{dirPath}");

                        // copy to temp directory
                        foreach (var filePath in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
                        {
                            var extension = Path.GetExtension(filePath);
                            if (excludeFileExtensions != null && excludeFileExtensions.Contains(extension))
                            {
                                continue;
                            }

                            var relativePath = filePath.Substring(dirName.Length + 1);
                            var targetFilePath = Path.Combine(tempDirectoryPath, relativePath);

                            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                            File.Copy(filePath, targetFilePath, true);
                        }
                    }
                }

                var dupCount = 0;

                while (File.Exists(destinationZipFilePath))
                {
                    ++dupCount;

                    var extension = Path.GetExtension(destinationZipFilePath).Trim();
                    var withoutExtension = destinationZipFilePath.Substring(0, destinationZipFilePath.IndexOf(extension));
                    if (withoutExtension.Length > 2 && int.TryParse(withoutExtension.Substring(withoutExtension.Length - 1), out var num))
                    {
                        withoutExtension = withoutExtension.Substring(0, withoutExtension.Length - 2);
                    }

                    destinationZipFilePath = $"{withoutExtension}_{dupCount}{extension}";
                }

                ZipFile.CreateFromDirectory(tempDirectoryPath, destinationZipFilePath);
                Logger.Log("Directory zipped successfully: " + destinationZipFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            finally
            {
                // clean up
                if (Directory.Exists(tempDirectoryPath))
                {
                    Directory.Delete(tempDirectoryPath, true);
                }
            }

            return false;
        }
    }
}