# net-keyence-hostlink

基于 [node-keyence-hostlink](https://github.com/chnbohwr/keyence-hostlink) 开发的 C# 版本基恩士 (Keyence) PLC Hostlink 上位链路通信协议库。

## 特性

- **高效**: 基于 Task 异步编程模型，零阻塞通信
- **低占用**: 最小化内存分配，对象池复用
- **兼容性强**: 核心库基于 .NET Standard 2.0，兼容 .NET Framework 4.6.1+
- **渐进增强**: .NET 6+ 使用 `ArrayPool<T>`、`Span<T>`、`ValueTask` 等现代特性
- **稳定性优先**: 工业级容错、自动重连、心跳检测

## 项目结构

```
net-keyence-hostlink/
├── src/
│   ├── Keyence.HostLink/                    # 核心库 (.NET Standard 2.0)
│   ├── Keyence.HostLink.Net6/               # .NET 6+ 增强库
│   └── Keyence.HostLink.Serial/             # 串口扩展库
├── tests/
│   ├── Keyence.HostLink.Tests.WinForms/     # WinForm 测试工具 (.NET Framework 4.7.2)
│   └── Keyence.HostLink.Tests.Net8.WinForms/ # WinForm 测试工具 (.NET 8)
├── examples/
│   └── ConsoleExample/                      # 控制台示例 (.NET 6)
└── Keyence.HostLink.sln                     # 解决方案
```

## 快速开始

### 基本使用

```csharp
using Keyence.HostLink;

var options = new HostLinkOptions
{
    Host = "192.168.3.100",
    Port = 8501,
    ConnectTimeout = TimeSpan.FromSeconds(5),
    AutoReconnect = true
};

using var client = new HostLinkClient(options);

// 连接
await client.ConnectAsync();

// 读取 DM0
var result = await client.ReadItemAsync("DM0");
Console.WriteLine($"DM0 = {result.ToInt16()}");

// 写入 DM0 = 100
await client.WriteItemAsync("DM0", "100");

// 批量读取 DM0-DM9
var batchResult = await client.ReadContinuousAsync("DM0", 10);
var values = batchResult.ToIntArray();

// 断开连接
client.Disconnect();
```

### 支持的软元件

| 软元件 | 前缀 | 类型 | 说明 |
|--------|------|------|------|
| Data Memory | DM | 16-bit Word | 数据寄存器 |
| Relay | R | Bit | 内部继电器 |
| Link Relay | LR | Bit | 链接继电器 |
| Internal Aux | MR | Bit | 内部辅助继电器 |
| Timer | T | Bit | 定时器触点/线圈 |
| Counter | C | Bit | 计数器触点/线圈 |
| Current Timer | TC | 16-bit | 定时器当前值 |
| Current Counter | CC | 16-bit | 计数器当前值 |
| Extended Memory | EM | 16-bit | 扩展数据寄存器 |

### 配置选项

```csharp
var options = new HostLinkOptions
{
    Host = "192.168.3.100",           // PLC IP 地址
    Port = 8501,                       // Hostlink 端口 (默认 8501)
    ConnectTimeout = TimeSpan.FromSeconds(5),  // 连接超时
    ReadTimeout = TimeSpan.FromSeconds(3),     // 读取超时
    WriteTimeout = TimeSpan.FromSeconds(3),    // 写入超时
    AutoReconnect = true,              // 自动重连
    ReconnectInterval = TimeSpan.FromSeconds(5), // 重连间隔
    MaxReconnectAttempts = -1,         // 最大重连次数 (-1 = 无限)
    EnableHeartbeat = false,           // 启用心跳
    HeartbeatInterval = TimeSpan.FromSeconds(30), // 心跳间隔
    StationNumber = 0,                 // 站号
    ReceiveBufferSize = 4096           // 接收缓冲区大小
};
```

### 事件处理

```csharp
client.Connected += (s, e) => Console.WriteLine("Connected to PLC");
client.Disconnected += (s, e) => Console.WriteLine("Disconnected from PLC");
```

## 构建

```bash
cd net-keyence-hostlink
dotnet build Keyence.HostLink.sln
```

## 运行测试

### WinForm 测试工具

在 Visual Studio 中打开解决方案，设置以下项目为启动项目：
- `Keyence.HostLink.Tests.WinForms` - .NET Framework 4.7.2 测试工具
- `Keyence.HostLink.Tests.Net8.WinForms` - .NET 8 测试工具

### 控制台示例

```bash
cd examples/ConsoleExample
dotnet run
```

## 渐进增强

| 特性 | .NET Framework 4.7.2 | .NET Standard 2.0 | .NET 6+ |
|------|---------------------|-------------------|---------|
| 基础通信 | BeginRead/EndRead | Task<T> | Task<T> |
| 异步 API | Callback/APM | TAP | TAP + await foreach |
| 缓冲区复用 | 手动管理 | ArraySegment<T> | ArrayPool<T> + Memory<T> |
| 值任务 | 不支持 | 不支持 | ValueTask<T> |
| 字符串优化 | string | string | Span<char> / string.Create |

## 协议说明

### 命令格式

```
读命令: RD <address>\r\n
写命令: WR <address> <value>\r\n
连续读: RCS <start_addr> <count>\r\n
连续写: WCS <start_addr> <count> <values...>\r\n
```

### 响应格式

成功响应返回数据值，失败响应返回错误代码：

| 错误代码 | 说明 |
|----------|------|
| ?01 | 命令格式错误 |
| ?02 | 命令错误 |
| ?03 | 地址错误 |
| ?04 | 数据范围错误 |
| ?10 | 不允许写入 |

## 参考资料

- [node-keyence-hostlink](https://github.com/chnbohwr/keyence-hostlink) - 原始 Node.js 实现
- [Keyence KV Series Manuals](https://www.keyence.com/support/user/controls/plc/manual/) - 官方协议文档

## License

MIT
