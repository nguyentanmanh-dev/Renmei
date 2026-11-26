using Renmei.Application.Abstractions;

namespace Renmei.Application.Tests.Support;

internal sealed class StubCurrentUser : ICurrentUser
{
    public static readonly Guid FixedUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public Guid? UserId => FixedUserId;
}