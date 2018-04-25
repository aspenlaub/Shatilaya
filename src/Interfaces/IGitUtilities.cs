﻿using System;
using LibGit2Sharp;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IGitUtilities {
        /// <summary>
        /// Given a git check-out folder, what is the friendly branch name?
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        string CheckedOutBranch(IFolder folder);

        /// <summary>
        /// Cover of Repository.Clone
        /// </summary>
        /// <param name="url"></param>
        /// <param name="folder"></param>
        /// <param name="cloneOptions"></param>
        /// <param name="useCache"></param>
        /// <param name="errorsAndInfos"></param>
        void Clone(string url, IFolder folder, CloneOptions cloneOptions, bool useCache, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Cover of Repository.Clone
        /// </summary>
        /// <param name="url"></param>
        /// <param name="folder"></param>
        /// <param name="cloneOptions"></param>
        /// <param name="useCache"></param>
        /// <param name="extraCacheCondition"></param>
        /// <param name="onCloned"></param>
        /// <param name="errorsAndInfos"></param>
        void Clone(string url, IFolder folder, CloneOptions cloneOptions, bool useCache, Func<bool> extraCacheCondition, Action onCloned, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Return the sha of the latest commit
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <returns></returns>
        string HeadTipIdSha(IFolder repositoryFolder);

        /// <summary>
        /// Verify that the local repository does not contain uncommitted changes
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <param name="errorsAndInfos"></param>
        /// <returns></returns>
        void VerifyThatThereAreNoUncommittedChanges(IFolder repositoryFolder, IErrorsAndInfos errorsAndInfos);

        /// <summary>
        /// Check if checked out branch is ahead of the master
        /// </summary>
        /// <param name="repositoryFolder"></param>
        /// <returns></returns>
        bool IsBranchAheadOfMaster(IFolder repositoryFolder);
    }
}
