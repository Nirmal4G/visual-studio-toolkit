﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Community.VisualStudio.Toolkit
{
    /// <summary>
    /// Represents a project in the solution hierarchy.
    /// </summary>
    public class Project : SolutionItem
    {
        private ReferenceCollection? _references;

        internal Project(IVsHierarchyItem item, SolutionItemType type) : base(item, type)
        { ThreadHelper.ThrowIfNotOnUIThread(); }

        /// <summary>
        /// Starts a build, rebuild, or clean of the project.
        /// </summary>
        public Task BuildAsync(BuildAction action = BuildAction.Build)
        {
            return VS.Build.BuildProjectAsync(this, action);
        }

        /// <summary>
        /// Adds one or more files to the project.
        /// </summary>
        public async Task<IEnumerable<PhysicalFile>> AddExistingFilesAsync(params string[] filePaths)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            GetItemInfo(out IVsHierarchy hierarchy, out uint itemId, out _);

            VSADDRESULT[] result = new VSADDRESULT[filePaths.Count()];
            IVsProject ip = (IVsProject)hierarchy;

            ErrorHandler.ThrowOnFailure(ip.AddItem(itemId, VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE, string.Empty, (uint)filePaths.Count(), filePaths, IntPtr.Zero, result));

            List<PhysicalFile> files = new();

            foreach (string filePath in filePaths)
            {
                PhysicalFile? file = await PhysicalFile.FromFileAsync(filePath);

                if (file != null)
                {
                    files.Add(file);
                }
            }

            return files;
        }

        /// <summary>
        /// References in the project.
        /// </summary>
        public ReferenceCollection References => _references ??= new(this);

        /// <summary>
        /// Checks what kind the project is.
        /// </summary>
        /// <param name="typeGuid">Use the <see cref="ProjectTypes"/> collection for known GUIDs.</param>
        public async Task<bool> IsKindAsync(string typeGuid)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            GetItemInfo(out IVsHierarchy hierarchy, out _, out _);

            return hierarchy.IsProjectOfType(typeGuid);
        }

        /// <summary>
        /// Save the project if it's dirty.
        /// </summary>
        public async Task SaveAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            GetItemInfo(out IVsHierarchy hierarchy, out _, out _);

            IVsSolution solution = await VS.Services.GetSolutionAsync();
            int hr = solution.SaveSolutionElement((uint)__VSSLNSAVEOPTIONS.SLNSAVEOPT_SaveIfDirty, hierarchy, 0);

            ErrorHandler.ThrowOnFailure(hr);
        }

        /// <summary>
        /// Tries to set an attribute in the project file for the item.
        /// </summary>
        public async Task<bool> TrySetAttributeAsync(string name, string value)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            GetItemInfo(out IVsHierarchy hierarchy, out _, out _);

            if (hierarchy is IVsBuildPropertyStorage storage)
            {
                storage.SetPropertyValue(name, "", (uint)_PersistStorageType.PST_PROJECT_FILE, value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to retrieve an attribute value from the project file for the item.
        /// </summary>
        /// <returns><see langword="null"/> if the attribute doesn't exist.</returns>
        public async Task<string?> GetAttributeAsync(string name)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            GetItemInfo(out IVsHierarchy hierarchy, out _, out _);

            if (hierarchy is IVsBuildPropertyStorage storage)
            {
                storage.GetPropertyValue(name, "", (uint)_PersistStorageType.PST_PROJECT_FILE, out string? value);
                return value;
            }

            return null;
        }
    }
}
