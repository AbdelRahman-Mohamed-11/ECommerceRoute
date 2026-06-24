namespace ECommerce.Domain.Shared;

public class Result
{
    protected Result(bool isSuccess, Error error, SuccessType successType)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException(
                "Successful result cannot contain an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException(
                "Failed result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
        SuccessType = successType;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public SuccessType SuccessType { get; }

    public static Result Success() =>
        new(true, Error.None, SuccessType.Ok);

    public static Result Created() =>
        new(true, Error.None, SuccessType.Created);

    public static Result NoContent() =>
        new(true, Error.None, SuccessType.NoContent);

    public static Result Failure(Error error) =>
        new(false, error, SuccessType.Ok);
}
