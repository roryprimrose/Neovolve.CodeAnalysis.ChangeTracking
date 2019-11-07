
$sourcesDirectory = $PSScriptRoot

$data = Get-Content "$sourcesDirectory/version.txt"

Remove-Item "$sourcesDirectory/version.txt"

$nuGetVersion = $data[0]
$assemblySemVer = $data[1]
$majorMinorPatch = $data[2]
$informationalVersion = $data[3]

if ($nuGetVersion -eq $null) {
    Write-Error ("GitVersion_NuGetVersionV2 environment variable is missing.")
    exit 1
}

if ($assemblySemVer -eq $null) {
    Write-Error ("GitVersion_AssemblySemVer environment variable is missing.")
    exit 1
}

if ($majorMinorPatch -eq $null) {
    Write-Error ("GitVersion_MajorMinorPatch environment variable is missing.")
    exit 1
}

if ($informationalVersion -eq $null) {
    Write-Error ("GitVersion_InformationalVersion environment variable is missing.")
    exit 1
}

Function Set-NodeValue($rootNode, [string]$nodeName, [string]$value)
{   
    $nodePath = "PropertyGroup/$($nodeName)"
    
    $node = $rootNode.Node.SelectSingleNode($nodePath)

    if ($node -eq $null) {
        Write-Output "Adding $($nodeName) element to existing PropertyGroup"

        $group = $rootNode.Node.SelectSingleNode("PropertyGroup")
        $node = $group.OwnerDocument.CreateElement($nodeName)
        $group.AppendChild($node) | Out-Null
    }

    $node.InnerText = $value

    Write-Output "Set $($nodeName) to $($value)"
}

Write-Output "Updating csproj files with the following version information"
Write-Output "GitVersion_NuGetVersionV2: $nuGetVersion"
Write-Output "GitVersion_AssemblySemVer: $assemblySemVer"
Write-Output "GitVersion_MajorMinorPatch: $majorMinorPatch"
Write-Output "GitVersion_InformationalVersion: $informationalVersion"
Write-Output ""
Write-Output "Searching for projects under $sourcesDirectory"
Write-Output ""

# This code snippet gets all the files in $Path that end in ".csproj" and any subdirectories.
Get-ChildItem -Path $sourcesDirectory -Filter "*.csproj" -Recurse -File | 
    ForEach-Object { 
        
        Write-Output "Found project at $($_.FullName)"

        $projectPath = $_.FullName
        $project = Select-Xml $projectPath -XPath "//Project"
        
        Set-NodeValue $project "Version" $nuGetVersion
        Set-NodeValue $project "AssemblyVersion" $assemblySemVer
        Set-NodeValue $project "FileVersion" $majorMinorPatch
        Set-NodeValue $project "InformationalVersion" $informationalVersion 

        $document = $project.Node.OwnerDocument
        $document.PreserveWhitespace = $true

        $document.Save($projectPath)

        Write-Output ""
    }
