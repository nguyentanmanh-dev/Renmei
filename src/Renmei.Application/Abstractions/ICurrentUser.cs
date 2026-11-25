namespace Renmei.Application.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }
}