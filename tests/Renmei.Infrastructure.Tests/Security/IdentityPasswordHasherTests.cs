using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Renmei.Application.Abstractions;
using Renmei.Infrastructure.Security;

namespace Renmei.Infrastructure.Tests.Security;

public sealed class IdentityPasswordHasherTests
{
    private static IdentityPasswordHasher CreateHasher(int iterations = 10_000)
        => new(Options.Create(new PasswordHasherOptions { IterationCount = iterations }));

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public void Hash_differs_from_plaintext_and_uses_unique_salt_each_time()
    {
        var sut = CreateHasher();
        var h1 = sut.Hash("MatKhauAnToan123");
        var h2 = sut.Hash("MatKhauAnToan123");

        Assert.NotEqual("MatKhauAnToan123", h1);
        Assert.NotEqual(h1, h2);
    }

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public void Verify_handles_valid_wrong_empty_and_unicode_passwords()
    {
        var sut = CreateHasher();
        const string unicodePwd = "日本語パスワード-tiếng-Việt-🔐";
        var hash = sut.Hash(unicodePwd);

        Assert.Equal(PasswordCheck.Success, sut.Verify(hash, unicodePwd));
        Assert.Equal(PasswordCheck.Failed, sut.Verify(hash, "sai-mat-khau"));
        Assert.Equal(PasswordCheck.Failed, sut.Verify(hash, string.Empty));
    }

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public void Verify_returns_SuccessRehashNeeded_when_iteration_count_increases()
    {
        var oldHasher = CreateHasher(iterations: 1_000);
        var newHasher = CreateHasher(iterations: 50_000);

        var oldHash = oldHasher.Hash("MatKhauCu123");

        Assert.Equal(PasswordCheck.SuccessRehashNeeded, newHasher.Verify(oldHash, "MatKhauCu123"));
    }

    [Fact]
    [Trait("Requirement", "NFR-002")]
    public void Hash_and_Verify_support_maximum_128_character_password()
    {
        var sut = CreateHasher();
        var maxPwd = new string('p', 128);
        var hash = sut.Hash(maxPwd);

        Assert.Equal(PasswordCheck.Success, sut.Verify(hash, maxPwd));
    }
}