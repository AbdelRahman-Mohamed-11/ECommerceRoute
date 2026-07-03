namespace ECommerce.Domain.Shared;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException(
                "Successful result cannot contain an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException(
                "Failed result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() =>
        new(true, Error.None);
    public static Result Failure(Error error) =>
        new(false, error);
}
