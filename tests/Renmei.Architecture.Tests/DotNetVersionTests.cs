using System.Text.Json;
using System.Xml.Linq;

namespace Renmei.Architecture.Tests;

public class DotNetVersionTests
{
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Renmei.Backend.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Không tìm thấy Renmei.Backend.slnx");
    }

    [Fact]
    public void Every_backend_project_targets_net10()
    {
        var root = RepoRoot();
        var wrong = new List<string>();

        foreach (var p in new[] { "src", "tests" }.SelectMany(d =>
                     Directory.EnumerateFiles(Path.Combine(root, d), "*.csproj", SearchOption.AllDirectories)))
        {
            var tfm = XDocument.Load(p).Descendants("TargetFramework").Select(e => e.Value.Trim()).FirstOrDefault();
            if (tfm != "net10.0")
            {
                wrong.Add($"{Path.GetFileName(p)}: {tfm ?? "(không có)"}");
            }
        }

        Assert.True(wrong.Count == 0, string.Join(Environment.NewLine, wrong));
    }

    [Fact]
    public void GlobalJson_pins_dotnet_10_sdk()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(RepoRoot(), "global.json")));
        var sdk = doc.RootElement.GetProperty("sdk");
        var version = sdk.GetProperty("version").GetString();
        var allowPrerelease = sdk.GetProperty("allowPrerelease").GetBoolean();

        Assert.StartsWith("10.", version);
        Assert.False(allowPrerelease);
    }
}