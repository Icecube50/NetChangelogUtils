# NetChangelogUtils

A tag-driven command line changelog and versioning tool

This tool:

-   Discovers all `.csproj` files
-   Retrieves project information
-   Parses commit messages
-   Evaluates which projects have been updated
-   Generates `CHANGELOG.md` per project
-   Updates project version information
-   Creates release tags

Designed for solutions containing multiple projects that need to be versioned independently.

------------------------------------------------------------------------

## Project setup

The tool works by analyzing the following nodes in the .csproj xml tree

```xml
<PropertyGroup>
    <Version>1.1.0</Version>
    <FileVersion>1.1.0</FileVersion>
    <AssemblyVersion>1.1.0</AssemblyVersion>
    <Product>ChangelogUtils</Product>
</PropertyGroup>
```

At least one version tag is needed, if none are configured the project will be ignored.
Each configured version will be updated by the tool.

The product tag is optional and only used so that the changelog and tags may be customized. If none is configured it fallbacks to the project name.

------------------------------------------------------------------------

## Commit Message Format

Commits must follow this format:

Category<Scope> Description

Examples:

```
  Feat<ToolA> Add caching layer
  Fix<ToolA, ToolB> Fix null reference issue
  Break<ToolA> Remove deprecated API
  Docs Update README
```


Rules:

-   `Category` must be defined in config
-   `Scope` must be a valid product name or alias
-   Multiple scopes are comma-separated
-   If no scope is provided, the change applies to all products
-   Multiline commit messages are supported

------------------------------------------------------------------------

## Tag Format

Tags are created per product using:

ProductName_vX.Y.Z

Example:

```
  ToolA_v1.2.0
  ToolB_v3.4.1
```

Multiple tags may point to the same commit.

------------------------------------------------------------------------

## Configuration

The tool will try to load the config from `.changelogUtils` by default.\
Bump may be: None, Patch, Minor or Major

```json
{
    "versioning": {
        "defaultStrategy": "Patch",
        "keywords": [
            {
                "keyword": "Feat",
                "bump": "Minor",
                "changelogSection": "Features"
            },
            {
                "keyword": "Fix",
                "bump": "Patch",
                "changelogSection": "Bug Fixes"
            },
            {
                "keyword": "Break",
                "bump": "Major",
                "changelogSection": "Breaking Changes"
            },
            {
                "keyword": "Docs",
                "bump": "None",
                "changelogSection": "Documentation"
            }
        ]
    },
    "scopeAliases": [
        {
            "alias": "Core",
            "products": [
                "ToolA",
                "ToolB"
            ]
        },
        {
            "alias": "UI",
            "products": [
                "ToolB" 
            ]
        }
    ]
}
```


------------------------------------------------------------------------

## Usage

Run from any directory inside the repository:

```
  NetChangelogUtils [Options]
```

The tool automatically detects the Git repository root.

### Options

| Flag               | Description                                       |
| ------------------ | ------------------------------------------------- |
| \--dry-run         | Use to preview changes made by the tool<br>       |
| \--ignore-unscoped | Do not track unscoped entries                     |
| \--project [path]  | Use to manually set path to the project directory |
| \--config [path]   | Use to manually set path to the config file       |

------------------------------------------------------------------------
