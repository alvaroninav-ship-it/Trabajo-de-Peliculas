using FluentValidation;
using Movies.Infrastructure.DTOs;
using Movies.Core.Enum;
using Movies.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Movies.Infrastructure.Validators
{
    public class SecurityDtoValidator : AbstractValidator<SecurityDto>
    {
        private readonly MoviesContext _context;

        public SecurityDtoValidator(MoviesContext context)
        {
            _context = context;

            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("El login es obligatorio")
                .MaximumLength(50).WithMessage("El login no puede exceder 50 caracteres")
                .MustAsync(async (login, cancellation) =>
                    !await _context.Securities.AnyAsync(s => s.Login == login))
                .WithMessage("El login ya está en uso");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("El rol especificado no es válido");
        }
    }
}
