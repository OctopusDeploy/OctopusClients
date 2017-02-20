using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Octopus.Cli.Util;

namespace Octopus.Cli.Tests.Util
{
    public class FakeOctopusFileSystem : IOctopusFileSystem
    {

        public Dictionary<string, string> Files { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> Deleted { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


        public bool FileExists(string path)
        {
            return Files.ContainsKey(path);
        }

        public bool DirectoryExists(string path)
        {
            throw new System.NotImplementedException();
        }

        public bool DirectoryIsEmpty(string path)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteFile(string path, DeletionOptions options)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDirectory(string path)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDirectory(string path, DeletionOptions options)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> EnumerateDirectories(string parentDirectoryPath)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> EnumerateDirectoriesRecursively(string parentDirectoryPath)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> EnumerateFiles(string parentDirectoryPath, params string[] searchPatterns)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> EnumerateFilesRecursively(string parentDirectoryPath, params string[] searchPatterns)
        {
            return searchPatterns
                .SelectMany(pattern => FindFilesEmulator(pattern, Files.Keys.ToArray()))
                .Distinct();
        }

        public long GetFileSize(string path)
        {
            throw new System.NotImplementedException();
        }

        public string ReadFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public void AppendToFile(string path, string contents)
        {
            throw new System.NotImplementedException();
        }

        public void OverwriteFile(string path, string contents)
        {
            throw new System.NotImplementedException();
        }

        public Stream OpenFile(string path, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
        {
            throw new System.NotImplementedException();
        }

        public Stream OpenFile(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite,
            FileShare share = FileShare.Read)
        {
            throw new System.NotImplementedException();
        }

        public Stream CreateTemporaryFile(string extension, out string path)
        {
            throw new System.NotImplementedException();
        }

        public string CreateTemporaryDirectory()
        {
            throw new System.NotImplementedException();
        }

        public void CopyDirectory(string sourceDirectory, string targetDirectory, int overwriteFileRetryAttempts = 3)
        {
            throw new System.NotImplementedException();
        }

        public void CopyDirectory(string sourceDirectory, string targetDirectory, CancellationToken cancel,
            int overwriteFileRetryAttempts = 3)
        {
            throw new System.NotImplementedException();
        }

        public ReplaceStatus CopyFile(string sourceFile, string destinationFile, int overwriteFileRetryAttempts = 3)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureDiskHasEnoughFreeSpace(string directoryPath)
        {
            throw new System.NotImplementedException();
        }

        public void EnsureDiskHasEnoughFreeSpace(string directoryPath, long requiredSpaceInBytes)
        {
            throw new System.NotImplementedException();
        }

        public string GetFullPath(string relativeOrAbsoluteFilePath)
        {
            throw new System.NotImplementedException();
        }

        public void OverwriteAndDelete(string originalFile, string temporaryReplacement)
        {
            throw new System.NotImplementedException();
        }

        public void WriteAllBytes(string filePath, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public string RemoveInvalidFileNameChars(string path)
        {
            throw new System.NotImplementedException();
        }

        public void MoveFile(string sourceFile, string destinationFile)
        {
            throw new System.NotImplementedException();
        }

        public ReplaceStatus Replace(string path, Stream stream, int overwriteRetryAttempts = 3)
        {
            throw new System.NotImplementedException();
        }

        public bool EqualHash(Stream first, Stream second)
        {
            throw new System.NotImplementedException();
        }

        public string ReadAllText(string scriptFile)
        {
            throw new System.NotImplementedException();
        }

        public string[] FindFilesEmulator(string pattern, string[] names)
        {
            List<string> matches = new List<string>();
            Regex regex = FindFilesPatternToRegex.Convert(pattern);
            foreach (string s in names)
            {
                if (regex.IsMatch(s))
                {
                    matches.Add(s);
                }
            }
            return matches.ToArray();
        }

        internal static class FindFilesPatternToRegex
        {
            private static Regex HasQuestionMarkRegEx = new Regex(@"\?", RegexOptions.Compiled);
            private static Regex IllegalCharactersRegex = new Regex("[" + @"\/:<>|" + "\"]", RegexOptions.Compiled);
            private static Regex CatchExtentionRegex = new Regex(@"^\s*.+\.([^\.]+)\s*$", RegexOptions.Compiled);
            private static string NonDotCharacters = @"[^.]*";
            public static Regex Convert(string pattern)
            {
                if (pattern == null)
                {
                    throw new ArgumentNullException();
                }
                pattern = pattern.Trim();
                if (pattern.Length == 0)
                {
                    throw new ArgumentException("Pattern is empty.");
                }
                if (IllegalCharactersRegex.IsMatch(pattern))
                {
                    throw new ArgumentException("Pattern contains illegal characters.");
                }
                bool hasExtension = CatchExtentionRegex.IsMatch(pattern);
                bool matchExact = false;
                if (HasQuestionMarkRegEx.IsMatch(pattern))
                {
                    matchExact = true;
                }
                else if (hasExtension)
                {
                    matchExact = CatchExtentionRegex.Match(pattern).Groups[1].Length != 3;
                }
                string regexString = Regex.Escape(pattern);
                regexString = "^" + Regex.Replace(regexString, @"\\\*", ".*");
                regexString = Regex.Replace(regexString, @"\\\?", ".");
                if (!matchExact && hasExtension)
                {
                    regexString += NonDotCharacters;
                }
                regexString += "$";
                Regex regex = new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return regex;
            }
        }
    }
}