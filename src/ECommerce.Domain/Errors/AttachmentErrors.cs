using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Errors;

public static class AttachmentErrors
{
    public static readonly Error EmptyFile =
        Error.Validation(
            "Attachment.EmptyFile",
            "File is empty.");

    public static readonly Error UploadFailed =
        Error.Failure(
            "Attachment.UploadFailed",
            "Attachment upload failed.");
}
