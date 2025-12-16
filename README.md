# Remote .NET Debugging for F&S Boards running Linux

This VSCode configuration enables remote debugging of .NET applications via SSH on a F&S board running Linux.

## Prerequisites

### On Your Development Machine

- Visual Studio Code
- C# Dev Kit extension: `code --install-extension ms-dotnettools.csharp`
- .NET SDK 8.0 or higher
- SSH client (OpenSSH on Windows, or `ssh` on Linux)

### On Your F&S Board

- .NET Runtime 8.0 (for framework-dependent deployment) or none needed (for self-contained deployment)
- SSH server running
- vsdbg (Visual Studio Debugger) installed in the home directory

## Installing vsdbg on the F&S Board

On your board, run this command to download the GetVsDbg.sh script:

```bash
wget --no-check-certificate https://aka.ms/getvsdbgsh -O GetVsDbg.sh
```

Execute the script:

```bash
chmod +x GetVsDbg.sh
./GetVsDbg.sh -v vs2022 -l .vs-debugger/vs2022
```

This installs the debugger to `~/.vs-debugger/vs2022/vsdbg`.

## Configuration

### 1. Update settings.json

Edit `.vscode/settings.json` with your board's details:

```json
{
    "FUS": {
        "BOARD_IP": "10.0.0.56",                            // Your board's IP address
        "BOARD_ARCH": "arm64",                              // arm64 or arm
        "BOARD_USER": "root",                               // SSH username
        "BOARD_PATH": "/tmp",                               // Working directory on board
        "DOTNET_PATH": "/usr/bin/dotnet",                   // Path to dotnet runtime (framework-dependent only)
        "DEBUGGER_PATH": "~/.vs-debugger/vs2022/vsdbg",     // Path to the vsdbg installation
        "DISPLAY": ":0",                                    // Only needed for GUI Apps, find the right value with "echo $DISPLAY"
        "XDG_RUNTIME_DIR": "/run/user/0",                   // Only needed for GUI Apps, find the right value with "echo $XDG_RUNTIME_DIR"
        "APP_NAME": "hello_debug",                          // Your application name (must match .csproj)
        "DOTNET_VERSION": "net8.0",                         // The .NET version defined in .csproj
        "SSH_TOOL": "ssh",
        "SCP_TOOL": "scp"
    }
}
```

**Important:**

- Set `APP_NAME` to match your `.csproj` filename without the extension.
- When debugging GUI-Apps: set the right value for `DISPLAY` and `XDG_RUNTIME_DIR` (`echo $DISPLAY` / `echo $XDG_RUNTIME_DIR`)
- `SSH_TOOL` and `SCP_TOOL` will use system PATH by default
- If `ssh`/`scp` are not in your PATH, provide full paths (e.g., "C:\\Windows\\System32\\OpenSSH\\ssh.exe" on Windows)
- Set `BOARD_ARCH` based on your board's processor:  
  - `arm64` for 64-bit ARM (ARMv8, i.MX8 and i.MX9 boards)
  - `arm` for 32-bit ARM (ARMv7, i.MX6 and i.MX7 boards)

## Usage

### Build Only

Press `Ctrl+Shift+B` or run task: **build**

### Deploy and Run (Framework-Dependent)

Run task: **fus_launch-on-board**

- Requires .NET runtime on the board
- Smaller deployment size

### Deploy and Run (Self-Contained)

Run task: **fus_launch-self-contained-on-board**

- Includes .NET runtime in the deployment
- Larger deployment size
- Works without .NET runtime on board

### Debug (Framework-Dependent)

1. Press `F5` or select **Debug on FuS Board (.NET)**
2. Application deploys automatically and debugger attaches
3. Execution stops at entry point (configurable via `stopAtEntry`)
4. Works for both CLI and GUI applications

### Debug (Self-Contained)

1. Press `F5` or select **Debug on FuS Board (self-contained)**
2. Application deploys automatically and debugger attaches
3. Works without .NET runtime on the board
4. Works for both CLI and GUI applications

## Available Tasks

| Task | Description |
|------|-------------|
| `build` | Shortcut for `fus_build-debug-for-linux` (Ctrl+Shift+B) |
| `fus_build-debug-for-linux` | Builds framework-dependent debug binary |
| `fus_deploy-to-board` | Builds and deploys to board (framework-dependent) |
| `fus_launch-on-board` | Deploys and runs on board (framework-dependent) |
| `fus_build-self-contained-for-linux` | Builds self-contained debug binary |
| `fus_deploy-self-contained-to-board` | Builds and deploys self-contained binary |
| `fus_prepare-self-contained-for-debug` | Prepares self-contained binary for debugging (deploy + chmod) |
| `fus_launch-self-contained-on-board` | Deploys and runs self-contained binary |

## Cross-Platform Support

This configuration works on **Windows and Linux** out of the box. The tasks automatically detect your platform and use the appropriate path separators and commands.

### GUI Application Support

The debug configurations include environment variables that enable debugging of **GUI applications** (testet with [Avalonia UI](https://avaloniaui.net/)) in addition to CLI applications:

- `DISPLAY=:0` - Specifies the Wayland display (Typically `:0` if only one display is connected. This can even be a virtual display via RDP)
- `XDG_RUNTIME_DIR=/run/user/0` - Runtime directory for the user session (root user in this case)

### Platform-Specific Notes

**Windows**:

- Uses OpenSSH tools from `C:\Windows\System32\OpenSSH\` (if available in PATH)
- Alternative: Install Git for Windows which includes SSH tools
- If SSH tools are not in PATH, update `SSH_TOOL` and `SCP_TOOL` with full paths

**Linux**:

- SSH tools are typically pre-installed
- No additional configuration needed

## Further Reading

For more information, please see the [.NET on F&S Boards](https://www.fs-net.de/assets/download/docu/common/en/DOTNET%20on%20FS%20Boards.pdf) and [Linux on F&S Boards](https://www.fs-net.de/assets/download/docu/common/en/LinuxOnFSBoards_eng.pdf) documentations.
