namespace Renmei.Domain.Content;

public class ContentSource
{
    private ContentSource() { }

    public ContentSource(
        string name,
        string license,
        string? licenseUrl = null,
        string? attributionText = null,
        string? url = null,
        DateTime? licenseCheckedAtUtc = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(license)) throw new ArgumentException("License is required.", nameof(license));

        Name = name.Trim();
        License = license.Trim();
        LicenseUrl = licenseUrl;
        AttributionText = attributionText;
        Url = url;
        LicenseCheckedAt = licenseCheckedAtUtc;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string License { get; private set; } = null!;
    public string? LicenseUrl { get; private set; }
    public string? AttributionText { get; private set; }
    public string? Url { get; private set; }
    public DateTime? LicenseCheckedAt { get; private set; }
}