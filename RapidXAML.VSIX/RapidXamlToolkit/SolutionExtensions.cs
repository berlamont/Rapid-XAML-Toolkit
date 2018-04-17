﻿// <copyright file="SolutionExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace RapidXamlToolkit
{
    public static class SolutionExtensions
    {
        // Copied from EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder as can't use it directly
        private const string VsProjectKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

        public static IEnumerable<EnvDTE.Project> GetAllProjects(this EnvDTE.Solution solution)
        {
            var item = solution.Projects.GetEnumerator();

            while (item.MoveNext())
            {
                if (item.Current is EnvDTE.Project project)
                {
                    if (project.Kind == VsProjectKindSolutionFolder)
                    {
                        foreach (var subproj in project.GetSolutionFolderProjects())
                        {
                            yield return subproj;
                        }
                    }
                    else
                    {
                        yield return project;
                    }
                }
            }
        }

        public static IEnumerable<EnvDTE.Project> GetSolutionFolderProjects(this EnvDTE.Project solutionFolder)
        {
            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject != null)
                {
                    // If this is another solution folder, do a recursive call, otherwise add
                    if (subProject.Kind == VsProjectKindSolutionFolder)
                    {
                        foreach (var subproj in subProject.GetSolutionFolderProjects())
                        {
                            yield return subproj;
                        }
                    }
                    else
                    {
                        yield return subProject;
                    }
                }
            }
        }
    }
}
