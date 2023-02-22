using ChatApp.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Models.ModelValidators
{
    public class RegisterValidator : AbstractValidator<RegisterUserDTO>
    {
        public RegisterValidator(ChatAppDbContext dbContext)
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Login).Custom((val, context) => 
            {
                if(dbContext.Users.Any(l => l.Login == val))
                    {
                        context.AddFailure("Login", "User with this login already exists");
                    }
            }
            );
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Passwd).MinimumLength(4).Equal(e => e.ConfirmPasswd);
            

        }
    }
}
