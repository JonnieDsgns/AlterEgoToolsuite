# AlterEgoEditor - Currently Implemented Features

## Overview
AlterEgoEditor is a modding tool for F1 games that allows users to create, edit, and manage mod projects. This document outlines the features that are **currently implemented** in the AlterEgoEditor application (excluding EgoErpArchiver, which is a separate reference tool).

---

## 1. Project Management Features

### 1.1 Create New Project
- **Location**: `CreateProjectViewModel.cs`, `CreateProjectWindow.xaml`
- **Functionality**:
  - Create a new mod project with the following fields:
    - Project Name (required)
    - Author Name (required)
    - Game Selection from detected installations (required)
    - Mod Category selection (MyTeam, Tracks, Teams, Liveries, Helmets, Seasons)
    - Mod Icon upload (optional)
    - Screenshot uploads (optional, up to 5 slots)
    - Description (optional)
  - Automatic workspace initialization with folder structure:
    - `metadata/` - stores project.json and filelist.json
    - `userdata/` - stores mod icon and other user assets
    - `source/` - for mod source files
  - Project version automatically set to "0.1"

### 1.2 Open Project
- **Location**: `MainViewModel.cs`, `StartupViewModel.cs`
- **Functionality**:
  - Open existing `.aeproject` files via file dialog
  - Project files are ZIP archives containing workspace structure
  - Automatically extracts project to temporary workspace
  - Loads project configuration from internal `metadata/project.json`
  - Available from both startup screen and main menu

### 1.3 Save Project
- **Location**: `MainViewModel.cs`
- **Functionality**:
  - Save current project to existing `.aeproject` file
  - Updates internal `project.json` with current state
  - Compresses entire workspace into ZIP archive
  - Only available when a project is open
  - Shows confirmation message on success

### 1.4 Save Project As
- **Location**: `MainViewModel.cs`
- **Functionality**:
  - Save project to a new location with new filename
  - Default save location: `Documents/AlterEgoProjects/`
  - Automatically creates directory if it doesn't exist
  - Updates project file path after save
  - File filter: `.aeproject` files only

### 1.5 Close Project
- **Location**: `MainViewModel.cs`
- **Functionality**:
  - Prompts user to save unsaved changes before closing
  - Options: Yes (save and close), No (close without saving), Cancel
  - Clears editor state and returns to startup view
  - Only available when a project is open

---

## 2. Game Detection & Integration

### 2.1 Game Scanner Service
- **Location**: `GameScannerService.cs`
- **Functionality**:
  - Scans for installed F1 games on the system
  - Currently detects: F1 25 (Steam installation)
  - Returns game installation details:
    - Game Title
    - Root Path
    - Source (Steam/Epic/etc.)
    - Version Identifier
  - Automatic scan on startup

### 2.2 Game Installation Selection
- **Location**: `CreateProjectViewModel.cs`, `StartupViewModel.cs`
- **Functionality**:
  - Display list of detected game installations
  - User can select target game for their mod project
  - Project links to game's root directory path
  - Required for project creation

---

## 3. File System Browsing & Indexing

### 3.1 Game Files Index/Tree View
- **Location**: `IndexerService.cs`, `EditorViewModel.cs`, `EditorView.xaml`
- **Functionality**:
  - Recursive indexing of game installation directory
  - Hierarchical tree view displaying:
    - Folders
    - Files with type indicators (ERP, XML, DDS, PNG, PKG, etc.)
  - File type identification by extension:
    - binXML, dds, png, erp, pkg, dll, exe, dat, daf, cfg
  - Visual indicators:
    - Modified files shown in green (bold)
    - Folders with modified children shown in yellow
  - Resource type labels displayed for each file

### 3.2 ERP Archive Browsing
- **Location**: `ErpHandleManager.cs`, `IndexerService.cs`, `EditorViewModel.cs`
- **Functionality**:
  - ERP files can be selected in tree view
  - Automatic loading of ERP contents when selected
  - Display of resources within ERP archives
  - Resource information includes:
    - Resource Name
    - Resource Type (GfxSRVResource, GfxMaterialRes, GfxMeshRes, VTF, World, WoInstances, EventGraph, etc.)
    - Folder path within archive
    - Size (uncompressed)
    - Packed Size (compressed)
  - Singleton pattern for efficient ERP file handle management
  - Caches opened ERP files to avoid repeated disk reads

### 3.3 Resource List View
- **Location**: `EditorView.xaml`, `EditorViewModel.cs`
- **Functionality**:
  - DataGrid displaying resources from selected ERP file
  - Columns: Resource Name, Type
  - Read-only view
  - Single row selection
  - Custom dark theme styling
  - Auto-scroll support

---

## 4. User Interface Features

### 4.1 Startup Screen
- **Location**: `StartupView.xaml`, `StartupViewModel.cs`
- **Functionality**:
  - Welcome screen shown on application launch
  - Quick access buttons:
    - "New mod project" - Opens project creation dialog
    - "Open mod project" - Opens file browser for existing projects
  - External links section with buttons for:
    - AlterEgo Discord (https://discord.gg/R5dEjWXN)
    - NexusMods (https://www.nexusmods.com/games/f125)
    - Overtake.gg (https://www.overtake.gg/downloads/categories/f1-series.216/)
  - All external links open in default browser

### 4.2 Main Window
- **Location**: `MainWindow.xaml`, `MainViewModel.cs`
- **Functionality**:
  - Custom window chrome with rounded corners
  - Window control buttons (minimize, maximize, close)
  - Drag-movable title bar
  - Dynamic content area that switches between:
    - Startup view (when no project is open)
    - Editor view (when project is active)
  - Current project name displayed in top-right corner
  - Status bar showing:
    - Copyright: "AlterEgo Editor ©2026"
    - Status: "Currently in Pre-Alpha Status"

### 4.3 Menu System
- **Location**: `MainWindow.xaml`
- **Functionality**:
  - **File Menu**:
    - New Project
    - Open Project
    - Save
    - Save As
    - Export to Mod (placeholder - not implemented)
    - Close Project
    - Exit
  - **Edit Menu** (placeholders - not fully implemented):
    - Project Settings
    - Undo
    - Copy
    - Paste
  - **Help Menu**:
    - About AlterEgo Editor (placeholder)
    - Discord (functional link)
    - YouTube (placeholder)
    - FAQ (placeholder)

### 4.4 Editor View
- **Location**: `EditorView.xaml`, `EditorViewModel.cs`
- **Functionality**:
  - Split-pane layout with:
    - Left panel: Game file tree view (top) + Resource list (bottom)
    - Right panel: Content area for selected documents
    - Resizable splitters between panes
  - Toolbar with quick-access buttons (icons present but functionality not all implemented):
    - New Project
    - Open Project
    - Save
    - Import
    - Export
  - Dark theme UI throughout

### 4.5 Theme & Styling
- **Location**: `Themes/` directory, various XAML files
- **Functionality**:
  - Consistent dark theme (#FF161616 background)
  - Custom button styles:
    - StartMenuRoundedHoverButtonStyle
    - StartMenuLinkButtonsStyle
    - btnClose style (red hover effect)
  - Custom fonts: ZTNature font family
  - Fluent Icons (Segoe Fluent Icons font)

---

## 5. Data Models & Services

### 5.1 ModProject Model
- **Location**: `Core/Models/ModProject.cs`
- **Properties**:
  - GameTitle - Target game name
  - GameRootPath - Path to game installation
  - ProjectName - Mod project name
  - AuthorName - Mod author
  - ModCategory - Category classification
  - Description - Mod description
  - ProjectFilePath - Path to .aeproject file
  - ModVersion - Version number
  - ModIconPath - Path to mod icon
  - ScreenshotPaths - Collection of screenshot paths
  - TempWorkspacePath - Working directory
  - Computed paths: MetadataPath, UserdataPath, SourcePath
  - IsValid - Validation property

### 5.2 FileNode Model
- **Location**: `Core/Models/FileNode.cs`
- **Purpose**: Represents files and folders in game directory tree
- **Features**:
  - Hierarchical structure with parent-child relationships
  - Tracks modification status
  - Propagates modification status to parent folders

### 5.3 ResourceNode Model
- **Location**: `Core/Models/ResourceNode.cs`
- **Properties**:
  - Name - Resource filename
  - Uri - Resource identifier
  - ResourceType - Type classification
  - Folder - Internal folder path
  - Size - Uncompressed size
  - PackedSize - Compressed size
  - IsModified - Modification flag

### 5.4 GameInstallation Model
- **Location**: `Core/Models/GameInstallation.cs`
- **Purpose**: Represents detected game installation
- **Properties**:
  - GameTitle - Name of game
  - RootPath - Installation directory
  - Source - Install source (Steam, Epic, etc.)
  - VersionIdentifier - Game version

### 5.5 ProjectService
- **Location**: `Engine/Services/ProjectService.cs`
- **Functionality**:
  - InitializeProjectWorkspace - Creates workspace folder structure
  - WriteProjectConfigFile - Serializes project to JSON
  - ReadProjectConfigFile - Deserializes project from JSON
  - WriteProjectFile - Packages workspace into .aeproject ZIP
  - ReadProjectFile - Extracts .aeproject and loads configuration
  - Uses System.Text.Json for serialization
  - Uses System.IO.Compression for ZIP handling

### 5.6 IndexerService
- **Location**: `Engine/Services/IndexerService.cs`
- **Functionality**:
  - BuildIndex - Recursively crawls game directory
  - Creates FileNode tree structure
  - CrawlDirectory - Recursive directory traversal
  - CreateFileNode - File node factory
  - CreateErpNode - Special handling for ERP archives
  - GetDetailedResources - Retrieves resources from ERP file
  - GetResourceTypeFromExtension - File type identification
  - ClearIndex - Clears ERP file handles

### 5.7 ErpHandleManager
- **Location**: `Engine/Services/ErpHandleManager.cs`
- **Functionality**:
  - Singleton pattern for managing ERP file handles
  - GetErpContainer - Opens/caches ERP file readers
  - GetDetailedResources - Lists all resources in ERP
  - GetResource - Retrieves specific resource by identifier
  - ClearHandles - Releases all cached file handles
  - Uses EgoEngineLibrary.Archive.Erp for ERP parsing

---

## 6. Workspace System (Partially Implemented)

### 6.1 WorkspaceViewModel Base Class
- **Location**: `ViewModels/Workspaces/WorkspaceViewModel.cs`
- **Purpose**: Base class for document editors
- **Features**:
  - DisplayName - Tab/window title
  - FilePath - Path to file being edited
  - IsModified - Unsaved changes flag
  - IsLoading - Loading state indicator
  - Abstract Load() method for loading content
  - Virtual Close() method for cleanup
  - CloseWorkspace command
  - WorkspaceClosed event

### 6.2 TextureWorkspaceViewModel (Stub)
- **Location**: `ViewModels/Workspaces/TextureWorkspaceViewModel.cs`
- **Status**: File exists but is empty (not implemented)

### 6.3 ResourceViewModel
- **Location**: `ViewModels/Workspaces/ResourceViewModel.cs`
- **Status**: Exists as part of workspace system

### 6.4 TextureViewModel
- **Location**: `ViewModels/Workspaces/TextureViewModel.cs`
- **Status**: Exists as part of workspace system

---

## 7. Project File Format

### 7.1 .aeproject File Structure
- **Format**: ZIP archive with .aeproject extension
- **Contents**:
  ```
  ├── metadata/
  │   ├── project.json      (Project configuration)
  │   └── filelist.json     (List of modified files - currently empty array)
  ├── userdata/
  │   └── icon.*            (Mod icon image)
  ├── source/               (Modified game files - currently unused)
  ```

### 7.2 project.json Schema
- Serialized ModProject object containing all project properties
- JSON format with indented formatting for readability
- Includes all project metadata and paths

---

## 8. External Dependencies

### 8.1 EgoEngineLibrary
- **Purpose**: Library for parsing Ego Engine file formats
- **Used For**: 
  - ERP archive reading (ErpFile class)
  - Resource extraction and metadata
- **Note**: External dependency referenced in solution

### 8.2 CommunityToolkit.Mvvm
- **Purpose**: MVVM framework
- **Used For**:
  - ObservableObject base class
  - RelayCommand attribute
  - ObservableProperty attribute
  - Property change notifications

---

## Features NOT Yet Implemented

The following features appear in the UI but are **not implemented** or are **placeholders**:

1. **Export to Mod** - Menu item exists but no functionality
2. **Project Settings** - Menu item exists but no dialog
3. **Undo/Redo** - Menu items exist but no implementation
4. **Copy/Paste** - Menu items exist but no implementation
5. **About Dialog** - Menu item exists but no dialog
6. **YouTube Link** - Menu item exists but no URL
7. **FAQ** - Menu item exists but no content
8. **Import/Export Buttons** - Toolbar buttons exist but no functionality
9. **Resource Editing** - Can view but not edit resources
10. **Texture Viewer/Editor** - Workspace class exists but is empty
11. **Document Tabs** - Content area exists but no tab system
12. **File Extraction** - Can browse ERP contents but not extract
13. **File Replacement** - Cannot replace files in archives yet
14. **Mod Building** - Cannot build/package final mod yet
15. **File Change Tracking** - filelist.json exists but is not populated

---

## Summary

AlterEgoEditor currently provides a **solid foundation** for F1 game modding with these core capabilities:
- ✅ Complete project lifecycle (create, open, save, close)
- ✅ Game installation detection
- ✅ Comprehensive file system browsing
- ✅ ERP archive inspection
- ✅ Resource listing and metadata viewing
- ✅ Professional UI with dark theme
- ✅ Workspace management system architecture
- ✅ Project packaging (.aeproject format)

The application is in **Pre-Alpha status** with the infrastructure in place for future features like resource editing, texture viewing, file extraction/replacement, and mod building.
