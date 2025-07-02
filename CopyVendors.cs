using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Lombiq.Npm.Targets;

public class CopyVendors : Task
{
    /// <summary>
    /// Gets or sets the path of the .NET project.
    /// </summary>
    [Required]
    public string ProjectPath { get; set; }

    /// <summary>
    /// Gets or sets the name of the project file, located in <see cref="ProjectPath"/>.
    /// </summary>
    [Required]
    public string ProjectFileName { get; set; }

    public override bool Execute()
    {
        var document = XDocument.Load(Path.Combine(ProjectPath, ProjectFileName));
        var targetBase = Path.Combine(ProjectPath, "wwwroot", "vendors");

        if (!Directory.Exists(targetBase)) Directory.CreateDirectory(targetBase);

        foreach (var vendor in document.Root.XPathSelectElements("//ItemGroup/Vendor"))
        {
            FindVendor(ProjectPath, targetBase, vendor);
        }

        return true;
    }

    private void FindVendor(string projectPath, string targetBase, XElement vendor)
    {
        var path = Path.Combine(projectPath, vendor.Attributes("Include").First().Value);
        var name = Path.GetFileName(path);
        var target = Path.Combine(
            targetBase,
            vendor.XPathSelectElement("./CopyTo")?.Value?.Trim() is { Length: > 0 } copyTo ? copyTo : name);

        var patterns = vendor
                .Attributes("Pattern")
                .Select(item => item.Value.Trim())
                .Where(pattern => !string.IsNullOrEmpty(pattern))
                .ToList() is { Count: > 0 } customPatterns
            ? customPatterns
            : ["**\\*"];

        if (!Directory.Exists(path))
        {
            Log.LogError($"NPM package \"{name}\" does not exist (expected path: \"{path}\").");
            return;
        }

        if (Directory.Exists(target)) Directory.Delete(target, recursive: true);

        var subdirectories = vendor
            .XPathSelectElements("./Subdirectory")
            .Select(subdirectory => subdirectory.Value)
            .ToList();
        if (subdirectories.Count > 0)
        {
            foreach (var subdirectory in subdirectories)
            {
                var source = new DirectoryInfo(Path.Combine(path, subdirectory));
                if (source.Exists)
                {
                    CopyDirectory(source, target, patterns);
                }
                else
                {
                    Log.LogError(
                        $"NPM package \"{name}\" with subdirectory path \"{subdirectory}\" does not exist " +
                        $"(expected path: \"{path}\").");
                }
            }

            return;
        }

        if (Directory.Exists(Path.Combine(path, "dist")))
        {
            path = Path.Combine(path, "dist");
        }
        else
        {
            Log.LogWarning(
                $"NPM package item for \"{name}\" does not specify a \u003cSubdirectory\u003e{{value}}" +
                $"\u003c/Subdirectory\u003e child element and the default \"dist\" subdirectory does not " +
                $"exist. Please add a \u003cSubdirectory\u003e. Leave it empty if you want to copy the whole " +
                $"package directory.");
        }

        CopyDirectory(new(path), target, patterns);
    }

    private void CopyDirectory(DirectoryInfo source, string target, List<string> patterns)
    {
        Directory.CreateDirectory(target);

        var matcher = new Matcher();
        matcher.AddIncludePatterns(patterns);

        if (matcher.Execute(new DirectoryInfoWrapper(source)).Files.Select(file => file.Path).ToList() is not { Count: > 0 } files)
        {
            var plural = patterns.Count == 1 ? string.Empty : "s";
            var patternsText = string.Join(", ", patterns.Select(pattern => $"\"{pattern}\""));

            Log.LogError(
                $"Couldn't find any files that match the pattern{plural} {patternsText} in the directory " +
                $"\"{source.FullName}\".");
            return;
        }

        foreach (var file in files)
        {
            var destinationPath = Path.Combine(target, file);

            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
            File.Copy(Path.Combine(source.FullName, file), destinationPath, overwrite: true);
        }
    }
}
