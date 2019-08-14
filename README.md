### 使用AssetBundle.LoadFromStream和AssetBundle.LoadFromStream对Unity资源进行简单加密

测试的时候发现
```csharp
    public override int Read(byte[] array, int offset, int count)
```
AssetBundle在调用Stream.Read读取的时候会多次获取同一块内容 不知道是不是Unity的BUG
