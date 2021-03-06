﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibGit2FlowSharp.Enums;
using LibGit2Sharp;

namespace LibGit2FlowSharp.Tests
{
    internal static class TestHelpers
    {
        internal static void DeleteBranch(Repository repo, Branch branch)
        {
            if (branch == null)
                return;
            if (branch.IsCurrentRepositoryHead)
                repo.Checkout(repo.Branches[repo.Flow().GetPrefixByBranch(GitFlowSetting.Master)]);
            repo.Branches.Remove(branch);
        }

        internal static string CreateEmptyRepo(string basepath,string dirName = "emptyRepo", bool isBare = false)
        {
            var path = Path.Combine(basepath, dirName);
            Directory.CreateDirectory(path);
            Thread.Sleep(1000);
            Repository.Init(path,isBare);
            return path;
        }

        internal static void CreateLocalTestBranch(Repository repo, string branchName, GitFlowSetting setting, GitFlowRepoSettings repoConfig)
        {
            if (!string.IsNullOrWhiteSpace(branchName))
            {
                repoConfig.SetSetting(setting, branchName);
                repo.CreateBranch(branchName);
            }

        }

        internal static void CleanRepoDir(string basepath,string dirName = "emptyRepo")
        {
            var path = Path.Combine(basepath, dirName);
            DeleteReadOnlyDirectory(path);

        }

        public static void DeleteReadOnlyDirectory(string directory)
        {
            foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            {
                DeleteReadOnlyDirectory(subdirectory);
            }
            foreach (var fileName in Directory.EnumerateFiles(directory))
            {
                var fileInfo = new FileInfo(fileName);
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.Delete();
            }
            Directory.Delete(directory);
        }


        internal static Commit AddCommitToRepo(IRepository repository,Signature author = null)
        {
            if (author == null)
            {
                author = new Signature("Test", "UnitTest@xunit.com", DateTime.Now);
            }

            string random = Path.GetRandomFileName();
            string filename = random + ".txt";

            Touch(repository.Info.WorkingDirectory, filename, random);

            repository.Stage(filename);

            return repository.Commit("New commit", author, author);
        }

        internal static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        internal static string Touch(string parent, string file, string content = null, Encoding encoding = null)
        {
            string filePath = Path.Combine(parent, file);
            string dir = Path.GetDirectoryName(filePath);
            Debug.Assert(dir != null);

            Directory.CreateDirectory(dir);

            File.WriteAllText(filePath, content ?? string.Empty, encoding ?? Encoding.ASCII);

            return filePath;
        }

    }
}
