If you are experiencing issues with .NET SDK versions this is exactly what you need to fix it.

```bash
wget https://dot.net/v1/dotnet-install.sh -O ~/dotnet-install.sh
chmod +x ~/dotnet-install.sh

# Install .NET 8 SDK
~/dotnet-install.sh --version 8.0.415 --install-dir ~/.dotnet --architecture x64

# Install .NET 10 RC2 SDK
~/dotnet-install.sh --version 10.0.100-rc.2.25502.107 --install-dir ~/.dotnet --architecture x64
```
