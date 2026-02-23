namespace FinanceTracker.Application.Common;

public class NotFoundException : Exception
{
    public NotFoundException(string resource, Guid id)
        : base($"{resource} with id '{id}' was not found.") { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Access denied.") { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
