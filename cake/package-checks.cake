//////////////////////////////////////////////////////////////////////
// LISTS OF FILES USED IN CHECKING PACKAGES
//////////////////////////////////////////////////////////////////////

string[] ENGINE_FILES = {
        "nunit.engine.dll", "nunit.engine.core.dll", "nunit.engine.api.dll", "testcentric.engine.metadata.dll" };
string[] ENGINE_PDB_FILES = {
        "nunit.engine.pdb", "nunit.engine.core.pdb", "nunit.engine.api.pdb"};
string[] ENGINE_CORE_FILES = {
        "nunit.engine.core.dll", "nunit.engine.api.dll", "testcentric.engine.metadata.dll" };
string[] ENGINE_CORE_PDB_FILES = {
        "nunit.engine.core.pdb", "nunit.engine.api.pdb"};
string[] AGENT_FILES_NET20 = {
        "nunit-agent-net20.exe", "nunit-agent-net20.exe.config",
        "nunit-agent-net20-x86.exe", "nunit-agent-net20-x86.exe.config",
        "nunit.engine.core.dll", "nunit.engine.api.dll", "testcentric.engine.metadata.dll"};
string[] AGENT_FILES_NET40 = {
        "nunit-agent-net40.exe", "nunit-agent-net40.exe.config",
        "nunit-agent-net40-x86.exe", "nunit-agent-net40-x86.exe.config",
        "nunit.engine.core.dll", "nunit.engine.api.dll", "testcentric.engine.metadata.dll"};
string[] AGENT_FILES_NETCORE_3_1 = {
        "nunit-agent-netcore31.dll", "nunit-agent-netcore31.dll.config",
        "nunit.engine.core.dll", "nunit.engine.api.dll", "testcentric.engine.metadata.dll"};
string[] AGENT_FILES_NET_5_0 = {
        "nunit-agent-net50.dll", "nunit-agent-net50.dll.config",
        "nunit.engine.core.dll", "nunit.engine.api.dll", "testcentric.engine.metadata.dll"};
string[] AGENT_FILES_NET_6_0 = {
        "nunit-agent-net60.dll", "nunit-agent-net60.dll.config",
        "nunit.engine.core.dll", "nunit.engine.api.dll", "testcentric.engine.metadata.dll"};
string[] AGENT_PDB_FILES_NET20 = {
        "nunit-agent-net20.pdb", "nunit-agent-net20-x86.pdb", "nunit.engine.core.pdb", "nunit.engine.api.pdb"};
string[] AGENT_PDB_FILES_NET40 = {
        "nunit-agent-net40.pdb", "nunit-agent-net40-x86.pdb", "nunit.engine.core.pdb", "nunit.engine.api.pdb"};
string[] AGENT_PDB_FILES_NETCORE_3_1 = {
        "nunit-agent-netcore31.pdb", "nunit.engine.core.pdb", "nunit.engine.api.pdb"};
string[] AGENT_PDB_FILES_NET_5_0 = {
        "nunit-agent-net50.pdb", "nunit.engine.core.pdb", "nunit.engine.api.pdb"};
string[] AGENT_PDB_FILES_NET_6_0 = {
        "nunit-agent-net60.pdb", "nunit.engine.core.pdb", "nunit.engine.api.pdb"};
string[] CONSOLE_FILES = {
        "nunit-console.exe", "nunit-console.exe.config" };
string[] CONSOLE_FILES_NETCORE = {
        "nunit-console.exe", "nunit-console.dll", "nunit-console.dll.config" };

//////////////////////////////////////////////////////////////////////
// PACKAGE CHECK IMPLEMENTATION
//////////////////////////////////////////////////////////////////////

// NOTE: Package checks basically do no more than what the programmer might 
// do in opening the package itself and examining the content.

public bool CheckPackage(string package, params PackageCheck[] checks)
{
    Console.WriteLine("\nPackage Name: " + System.IO.Path.GetFileName(package));

    if (!FileExists(package))
    {
        WriteError("Package was not found!");
        return false;
    }

    if (checks.Length == 0)
    {
        WriteWarning("Package found but no checks were specified.");
        return true;
    }

    bool isMsi = package.EndsWith(".msi"); 
    string tempDir = isMsi
        ? InstallMsiToTempDir(package)
        : UnzipToTempDir(package);

    if (!System.IO.Directory.Exists(tempDir))
    {
        WriteError("Temporary directory was not created!");
        return false;
    }

    try
    {
        bool allPassed = ApplyChecks(tempDir, checks);
        if (allPassed)
            WriteInfo("All checks passed!");

        return allPassed;
    }
    finally
    {
        DeleteDirectory(tempDir, new DeleteDirectorySettings()
        {
            Recursive = true,
            Force = true
        });
    }
}

private string InstallMsiToTempDir(string package)
{
    // Msiexec does not tolerate forward slashes!
    package = package.Replace("/", "\\");
    var tempDir = GetTempDirectoryPath();
    
    WriteInfo("Installing to " + tempDir);
    int rc = StartProcess("msiexec", $"/a {package} TARGETDIR={tempDir} /q");
    if (rc != 0)
        WriteError($"Installer returned {rc.ToString()}");

    return tempDir;
}

private string UnzipToTempDir(string package)
{
    var tempDir = GetTempDirectoryPath();
 
    WriteInfo("Unzipping to " + tempDir);
    Unzip(package, tempDir);

    return tempDir;
}

private string GetTempDirectoryPath()
{
   return System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName() + "\\";
}

private bool ApplyChecks(string dir, PackageCheck[] checks)
{
    bool allOK = true;

    foreach (var check in checks)
        allOK &= check.Apply(dir);

    return allOK;
}

public abstract class PackageCheck
{
    public abstract bool Apply(string dir);
}

public class FileCheck : PackageCheck
{
    string[] _paths;

    public FileCheck(string[] paths)
    {
        _paths = paths;
    }

    public override bool Apply(string dir)
    {
        var isOK = true;

        foreach (string path in _paths)
        {
            if (!System.IO.File.Exists(dir + path))
            {
                WriteError($"File {path} was not found.");
                isOK = false;
            }
        }

        return isOK;
    }
}

public class DirectoryCheck : PackageCheck
{
    private string _path;
    private List<string> _files = new List<string>();

    public DirectoryCheck(string path)
    {
        _path = path;
    }

    public DirectoryCheck WithFiles(params string[] files)
    {
        _files.AddRange(files);
        return this;
    }

    public DirectoryCheck AndFiles(params string[] files)
    {
        return WithFiles(files);
    }

    public DirectoryCheck WithFile(string file)
    {
        _files.Add(file);
        return this;
    }

    public DirectoryCheck AndFile(string file)
    {
        return AndFiles(file);
    }

    public override bool Apply(string dir)
    {
        if (!System.IO.Directory.Exists(dir + _path))
        {
            WriteError($"Directory {_path} was not found.");
            return false;
        }

        bool isOK = true;

        if (_files != null)
        {
            foreach (var file in _files)
            {
                if (!System.IO.File.Exists(System.IO.Path.Combine(dir + _path, file)))
                {
                    WriteError($"File {file} was not found in directory {_path}.");
                    isOK = false;
                }
            }
        }

        return isOK;
    }
}

private FileCheck HasFile(string file) => HasFiles(new [] { file });
private FileCheck HasFiles(params string[] files) => new FileCheck(files);  

private DirectoryCheck HasDirectory(string dir) => new DirectoryCheck(dir);

private static void WriteError(string msg)
{
    Console.WriteLine("  ERROR: " + msg);
}

private static void WriteWarning(string msg)
{
    Console.WriteLine("  WARNING: " + msg);
}

private static void WriteInfo(string msg)
{
    Console.WriteLine("  " + msg);
}
