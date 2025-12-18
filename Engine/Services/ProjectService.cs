using Microsoft.Win32;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows; 
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;

using Core.Models;


namespace Engine.Services
{
    public class ProjectService
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true // For better readability
        };

        public void InitializeProjectWorkspace(ModProject project)
        {
            // 1. Define a unique temp path (e.g., in AppData/Local/AlterEgoEditor/Projects/ProjectName_GUID)
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string workspaceRoot = Path.Combine(appData, "AlterEgoEditor", "Workspaces",
                                                $"{project.ProjectName}_{Guid.NewGuid().ToString().Substring(0, 8)}");

            project.TempWorkspacePath = workspaceRoot;

            // 2. Create the physical folder structure
            Directory.CreateDirectory(project.MetadataPath); // metadata folder
            Directory.CreateDirectory(project.UserdataPath); // userdata folder
            Directory.CreateDirectory(project.SourcePath);   // source folder

            // 3. Create the initial project.json inside metadata
            string projectJsonPath = Path.Combine(project.MetadataPath, "project.json");
            WriteProjectConfigFile(project, projectJsonPath);

            // 4. Create the empty filelist.json for future modded files
            string fileListPath = Path.Combine(project.MetadataPath, "filelist.json");
            // We can just write an empty JSON array [] for now
            File.WriteAllText(fileListPath, "[]");
        }

        // Writes the ModProject object to a JSON file at the specified path
        public void WriteProjectConfigFile(ModProject project, string filePath)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project)); // Error handling
            }

            // 1. Serialize the object to a JSON string
            string jsonString = JsonSerializer.Serialize(project, _jsonOptions); // Serialize with options

            // 2. Write the string to the file path
            File.WriteAllText(filePath, jsonString); // Write to file
        }

        // Reads a ModProject object from a JSON file at the specified path
        public ModProject ReadProjectConfigFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The project file was not found.", filePath); // Error handling
            }

            string jsonString = File.ReadAllText(filePath); // Read the file content

            // Deserialize the JSON string back into a ModProject object
            var project = JsonSerializer.Deserialize<ModProject>(jsonString); // Deserialize

            return project ?? throw new InvalidOperationException("Could not deserialize project file."); // Null check and error handling
        }

        public void WriteProjectFile(ModProject project)
        {
            // 1. First, refresh the internal project.json inside the temp workspace
            string internalJsonPath = Path.Combine(project.MetadataPath, "project.json");
            WriteProjectConfigFile(project, internalJsonPath);

            // 2. Prepare to Pack: We use a temp file to avoid "file in use" errors during zipping
            string tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

            try
            {
                // 3. Compress the entire workspace folder into the temp ZIP
                ZipFile.CreateFromDirectory(project.TempWorkspacePath, tempZipPath);

                // 4. Move/Overwrite the final .aeproject file
                if (File.Exists(project.ProjectFilePath)) File.Delete(project.ProjectFilePath);
                File.Move(tempZipPath, project.ProjectFilePath);
            }
            finally
            {
                if (File.Exists(tempZipPath)) File.Delete(tempZipPath);
            }
        }

        public ModProject ReadProjectFile(string aeprojectPath)
        {
            // 1. Create a fresh session-based Temp Workspace
            string workspaceRoot = Path.Combine(Path.GetTempPath(), "AlterEgoEditor", "Workspaces",
                                                Guid.NewGuid().ToString().Substring(0, 8));

            if (Directory.Exists(workspaceRoot)) Directory.Delete(workspaceRoot, true);
            Directory.CreateDirectory(workspaceRoot);

            // 2. Extract the .aeproject (ZIP) into our workspace
            ZipFile.ExtractToDirectory(aeprojectPath, workspaceRoot);

            // 3. Load the config from the extracted metadata folder
            string internalJsonPath = Path.Combine(workspaceRoot, "metadata", "project.json");
            ModProject project = ReadProjectConfigFile(internalJsonPath);

            // 4. Update the object with its new physical locations
            project.TempWorkspacePath = workspaceRoot;
            project.ProjectFilePath = aeprojectPath;

            return project;
        }



    }
}
