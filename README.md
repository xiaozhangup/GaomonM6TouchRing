# Gaomon M6 滚动环和顶部软键

适用于原版 OpenTabletDriver 0.6.8.1。安装插件和随附的 `M6.json` 后，即可在原版界面配置滚动环和顶部 16 个软键。

## Linux 安装

从 [Releases](https://github.com/xiaozhangup/GaomonM6TouchRing/releases) 下载安装包 `GaomonM6TouchRing.zip`，解压并进入解压后的目录。将原始 ZIP 的路径传给 `otd installplugin`，作者与版本元数据会一并安装。

```sh
otd installplugin /path/to/GaomonM6TouchRing.zip
install -D -m 0644 M6.json "${XDG_CONFIG_HOME:-$HOME/.config}/OpenTabletDriver/Configurations/Gaomon/M6.json"
systemctl --user restart opentabletdriver
```

如果 OpenTabletDriver 已使用 `${XDG_DATA_HOME:-$HOME/.local/share}/OpenTabletDriver/Configurations` 或通过 `--config` 指定了目录，把 `M6.json` 放入实际使用的目录。已有名为 `Gaomon M6` 的自定义平板配置时，备份后把本包的 `Specifications.Wheels`、`Specifications.AuxiliaryButtons` 和对应设备标识里的 `ReportParser` 合并进去。保留一个同名配置。

重启后打开 OpenTabletDriver 的 **Wheel 1 Bindings**，分别给滚动环的顺时针和逆时针动作设置 **Mouse Scroll Binding**。每格对应 30°，两边的阈值均可设为 30°，滚动量设为 120，两边的 **Invert** 取值相反。也可为两个方向指定按键或其他动作。

## 顶部软键

打开原版界面的 **Auxiliary Settings**。**Auxiliary Binding 1–13** 对应原有按键，**Auxiliary Binding 14–29** 对应顶部软键。顶部软键从左到右编号 1–16，例如最左边对应 **Auxiliary Binding 14**，最右边对应 **Auxiliary Binding 29**。软键初始无绑定，用笔尖点按触发，抬笔释放。

软键带覆盖感应区的整个宽度，横向均分 16 格。纵向识别范围是板面顶端到 7 mm（原始 Y 坐标 0–1400）。只有在软键带内落笔才会触发软键，区外落笔和笔划继续由原有笔设置处理。

## 从源码构建

使用 .NET 10 SDK 运行 `dotnet build -c Release`。将生成的 `GaomonM6TouchRing.dll` 和仓库里的 `metadata.json` 放进 ZIP 根目录，再用 `otd installplugin` 安装。驱动会提供其余程序集。

## 卸载

移除本包安装的 `M6.json`，执行 `otd uninstallplugin GaomonM6TouchRing`，然后重启驱动。如果你合并过自定义配置，撤回该配置里的对应修改即可。

## 许可与来源

使用 LGPL-3.0-or-later。M6 配置基于 OpenTabletDriver 0.6.8.1，滚动环协议参考 mugaaaaa 的 [OpenTabletDriver PR #4984](https://github.com/OpenTabletDriver/OpenTabletDriver/pull/4984)。
