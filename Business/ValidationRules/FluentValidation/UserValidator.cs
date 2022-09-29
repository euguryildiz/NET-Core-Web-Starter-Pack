using System;
using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class UserValidator : AbstractValidator<User>
    {
       public UserValidator()
        {
            RuleFor(x => x.IsDeleted).NotNull();
            RuleFor(x => x.IsActive).NotNull();
            RuleFor(x => x.CreatedDate).NotNull();
            RuleFor(x => x.ModifiedDate).NotNull();
            RuleFor(x => x.CreatedUserId).NotNull();
            RuleFor(x => x.ModifiedUserId).NotNull();
            RuleFor(x => x.Name).MinimumLength(3);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}

