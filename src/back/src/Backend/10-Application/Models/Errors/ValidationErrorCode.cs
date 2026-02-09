namespace Application.Models.Errors;

public enum ValidationErrorCode
{
    Required = 0,
    MaxLength,
    Unique,
    Base64Format,
    InvalidPassword,
    PositiveNumber,
    GreaterThanZero,
    DocumentMustExist,
    UserMustExist,
    UserLinked,
    InvalidEmail,
    InvalidEnum,
    InvalidDate,
    CountryMustExist,
    InvalidSearchTerm,
    DefaultRoleCannotBeAltered,
    InvalidParent,
    GepgraphicAreaMustHaveParent,
    AlreadyExists,
}
