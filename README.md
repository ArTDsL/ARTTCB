# ART Tiny C Builder (ARTTCB) [![.NET](https://github.com/ArTDsL/ARTTCB/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/ArTDsL/ARTTCB/actions/workflows/dotnet.yml)
**ART Tiny C Builder** is a little **C Language** builder that uses GCC to perform quick project build based on pre-defined parameters.

Developed using [**C# NetCore**](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0) and [**YAML**](https://yaml.org/), it's easy to setup and build your project.

Download the binaries [**here**](https://github.com/ArTDsL/ARTTCB/releases)!

## Need to run

* [**GCC**](https://gcc.gnu.org/) _(Installed and configured in PATH variables)_;
* [**Visual Studio 2022**](https://visualstudio.microsoft.com/pt-br/vs/community/) or [**dotnet tools**](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install) _(if building)_;
* [**.NetCore 8**](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0) "_**Runtime**_" _(or "**SDK**" if building)_;

## How to use:

#### Using Binaries:

**Instructions here are the same for all systems (Windows / Linux / MacOS):**

**1)** _Copy `ARTTCB.exe` <small>(or `ARTTCBe` in case of **Linux** and **MacOs**)</small> to your project folder._

**2)** Create `buildme.tcb` file in the same folder you place **ARTTCB** executable;

**3)** Run `.\ARTTCB.exe -build -log` in a CMD <small>(or terminal `./ARTTCBe -build -log` in case of **Linux** and **MacOs**) to build directly with a `buildme.tcb`</small>.

**ARGUMENTS:** 
| **ARGUMENT**                | **WINDOWS**                                                            | **LINUX & MACOS**                                                        |
|-----------------------------|------------------------------------------------------------------------|--------------------------------------------------------------------------|
| `-build`                    | `.\ARTTCB -build`                                                      | `./ARTTCBe -build`                                                       |
| `-build --tcb "CUSTOM_TCB"` | `.\ARTTCB -build --tcb "CUSTOM_TCB"`                                   |`./ARTTCBe -build --tcb "CUSTOM_TCB"`                                     |
| `-log`                      | `.\ARTTCB -build -log` **~** `.\ARTTCB -build --tcb "CUSTOM_TCB" -log` | `./ARTTCBe -build -log` **~** `./ARTTCBe -build --tcb "CUSTOM_TCB" -log` |

> [!WARNING]  
> _**DO NOT PUT .TCB IN THE FILE NAME AFTER** `--tcb`;_ 

> [!IMPORTANT]  
> **OBS:** _**Make sure all [ARTTCB executable(s)](#using-binaries) are in the same folder, includer your `buildme.tcb`.**_


#### Compiling:

##### Windows:
**1)** In Windows open `ARTTCB.sln` (make sure you have [**Visual Studio 2022**](https://visualstudio.microsoft.com/pt-br/vs/community/) and [**.NetCore 8 SDK**](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0) installed).

**2)** Change on top from **Debug** to **Release**.

**3)** Compile first the lib project `ARTTCB` and after that the Console `ARTTCBConsole` project.

##### Linux and MacOs:

**1)** Make sure you have [**.NetCore 8 SDK**](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0) and [**dotnet tools**](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install) installed.

**2)** Open the folder where ARTTCB is located and open a terminal.

**3)** Use the command `dotnet build ARTTCB.sln`.

#### buildme.tcb

_File **buildme.tcb** is a [**YAML**](https://yaml.org/) file write with all information needed to build you project._

**Example buildme.tcb file:**<br>
_Modify as you need, DO NOT REMOVE ANY PARAMETER, if you don't want to use any info just put a "null" reference in front <small>(example: `next_build: null`)</small>;_
```yaml
# Project Info
project_name: ""
project_version: "1.0.0"
project_author: ""
project_buildname: ""
# Project Folders
working_dir: "C:\\example\\"
build_folder: "build\\" # Uses "working_dir" as ROOT.
include_folders: "includes\\" # Uses "working_dir" as ROOT.
code_main_dir: "src\\" # Uses "working_dir" as ROOT.
# "os"
os: WINDOWS #( WINDOWS | LINUX | MAC )
# "architecture"
architecture: x64 #( x64 | x86 | archx64 )
# Build type
build_type: DLIB #(SLIB: STATIC LIB | DLIB: Dynamic Lib | EXE: Windows Exec | LEXEC: Linux Exec | MEXEC: MacOS Exec")
# C Files
c_files:
- "File1.c"
- "Folder\\File2.c"
#
# GCC Compiler Extra Parameters
compiler_params:
- '-Wall'
- '-Wextra'
- '-Wformat-security'
- '-Werror'
- '-Wstack-protector'
- '-fstack-protector-strong'
- '-fstack-clash-protection'
- '-D_FORTIFY_SOURCE=2'
# Next Build
next_build: "buildme2"
# Auto create build folders (if you don't have)
auto_create_folders: true
# Generate a LOG File
generate_log: true
# Jump Object Compilation
jump_object_compilation: false
```

#### What means those parameters?

**Project Information:**
* **project\_name** - Name of the project (use only alpha-numeric with spaces, **can use uppercase and lowercase letters**).
* **project\_version** - Version of the project (Only numbers, dots and hyphens).
* **project\_author** - Author of the project (same as Project Name, but can include "**(c)**" for copyright, dots and commas.
* **project\_buildname** - This name will be used in file outputs and others (recommended only lowercase letters, alpha-numeric, separated only with underscore).

**Project Folders:**
* **working\_dir** - Full path where your project is located.
* **build\_folder** - Folder where your build files are going to be located;
* **include\_folders** - Folder where your include (headers) files are located;
* **code\_main\_dir** - Folder where your Source Code (\*.c) files are located;
    
    <small>(**OBS:** _Folders: **build\_folder**, **include\_folders**, **code\_main\_dir** have the **working\_dir** as **ROOT**, it means their path start inside the **working\_dir** folder._)</small>

**Operating System:**
* **os** - The **os** parameter tell **ARTTCB** what **Operating System** we are performing the build (in some cases gcc needs to know that) and also help organize your objects and binaries;
* * **WINDOWS** - Microsoft Windows
* * **LINUX** - Linux Based
* * **MAC** - Apple MacOS

**Architecture:**
* **architecture** - Architecture that **ARTTCB** will prompt gcc to build and also organize your objects and binaries;
* * **x64** - x64 architecture
* * **x86** - x86 (i86 A.K.A 32bits) architecture
* * **archx64** - New Arch-x64 architecture

**Build type:** _(This will change in the future as things goes...)_
* **build\_type** - The **build\_type** defines what type of build you want to perform:
* * **EXE** - Windows Executable
* * **LEXEC** - Linux Executable
* * **MEXEC** - MacOS Executable
* * **DLIB** - Dynamic Library
* * **SLIB** - Static Library

**C Files:**
* **c\_files** - Your *.c Files to build (Array) _(Also gonna change in the future)_;

**Compiler Params:**
* **compiler\_params** - All compile extra parameters goes here, it will be passed to GCC in the compilation line;

**Next Build:**
* **next\_build** - Need to build a Lib and then a Exe? Here is the solution, **next\_build** allows you to create multiple build stages for your app, just create another **buildme.tcb** file and drop the name here _(without **.tcb**)_, it will be executed one after another... In case of no next\_build keep it a **null** object.

**Extra:**
* **auto\_create\_folders** - Auto create build folder and all subfolders needed inside of it...
* **generate\_log** - Generate a log file in **%working\_dir%/ARTTCB\_LOGS/** folder.
* **jump\_object\_compilation** - Does not compile \*.o (objects) and go straight to main build (Lib or Exec).


## TODO:

- [ ] Linux Exec/Libs Build;
- [ ] MacOS Exec/Libs Build;
- [ ] Auto Parameters (some, not all);
- [x] Auto \*.c file referencer _(**ready, comming on 2.0.0**)_;
- [ ] Better options; 

## Finally...

* Thanks to [YamlDotNet](https://github.com/aaubry/YamlDotNet) for this amazing Nuget Package!

This project is not a HUGE builder like CMake or something, it's just something simple that i want to make/use. But feel free to open an issue (if you find one) or ask me anything!

My **X** is always open, [@ArT_DsL](https://www.x.com/ArT_DsL)!
