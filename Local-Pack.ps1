param (
    [Parameter(Mandatory=$true, HelpMessage="请输入版本号，例如: 0.4.0")]
    [string]$Version
)

# 1. 定义项目列表
$Projects = @(
    "MinionLib",
    "MinionLib.RitsuAdapters",
    "MinionLib.BaseLibAdapters"
)

Write-Host "=== 开始清理阶段 ===" -ForegroundColor Cyan

# 2. 执行 dotnet clean
Write-Host "正在执行 dotnet clean..." -ForegroundColor Gray
dotnet clean
if ($LASTEXITCODE -ne 0) {
    Write-Warning "dotnet clean 执行过程中出现警告或错误。"
}

# 3. 删除各项目中的 .godot 目录
foreach ($Proj in $Projects) {
    $GodotDir = Join-Path $Proj ".godot"
    if (Test-Path $GodotDir) {
        Write-Host "正在删除: $GodotDir" -ForegroundColor Gray
        Remove-Item -Path $GodotDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# 4. 执行 dotnet restore
Write-Host "正在执行 dotnet restore..." -ForegroundColor Gray
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet restore 执行失败，请检查项目依赖。"
    exit $LASTEXITCODE
}

Write-Host "`n=== 开始打包阶段 ===" -ForegroundColor Cyan

# 5. 执行打包命令
Write-Host "正在打包版本: $Version ..." -ForegroundColor White
# -p:Version 会覆盖项目文件中的版本号
dotnet pack -c ExportRelease "-p:Version=$Version"

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet pack 执行失败，请检查项目配置。"
    exit $LASTEXITCODE
}

# 6. 确保目标目录存在
$DestDir = ".local/nuget-$Version"
if (-not (Test-Path $DestDir)) {
    New-Item -ItemType Directory -Path $DestDir | Out-Null
    Write-Host "已创建本地输出目录: $DestDir" -ForegroundColor Gray
}

# 7. 拷贝生成的包 (.nupkg 和 .snupkg)
$Extensions = @("nupkg", "snupkg")

Write-Host "`n=== 开始拷贝包文件 ===" -ForegroundColor Cyan
foreach ($Proj in $Projects) {
    foreach ($Ext in $Extensions) {
        # 根据你提供的路径结构：项目名/.godot/mono/temp/bin/ExportRelease/文件名
        $FileName = "FuYnAloft.Sts2.$Proj.$Version.$Ext"
        $SourcePath = Join-Path $Proj ".godot\mono\temp\bin\ExportRelease\$FileName"

        if (Test-Path $SourcePath) {
            Copy-Item -Path $SourcePath -Destination $DestDir -Force
            Write-Host "成功 [OK]: $FileName" -ForegroundColor Green
        } else {
            Write-Warning "未找到 [SKIP]: $SourcePath"
        }
    }
}

Write-Host "`n全部流程执行完毕！" -ForegroundColor Cyan
