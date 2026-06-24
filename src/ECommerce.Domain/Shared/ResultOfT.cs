namespace ECommerce.Domain.Shared;

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    private Result(
        TValue? value,
        bool isSuccess,
        Error error,
        SuccessType successType)
        : base(isSuccess, error, successType)
    {
        _value = value;
    }

    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "Cannot access the value of a failed result.");

    public static Result<TValue> Success(TValue value) =>
        new(value, true, Error.None, SuccessType.Ok);

    public static Result<TValue> Created(TValue value) =>
        new(value, true, Error.None, SuccessType.Created);

    public static new Result<TValue> Failure(Error error) =>
        new(default, false, error, SuccessType.Ok);
}
