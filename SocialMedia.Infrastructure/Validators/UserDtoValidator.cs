using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.Data.Configurations;
using Movies.Infrastructure.DTOs;

namespace Movies.Infrastructure.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        private readonly MoviesContext _context;
        public UserDtoValidator(MoviesContext context)
        {
            _context = context;
            RuleFor(x => x.FirstName)
               .NotEmpty().WithMessage("El nombre es requerido")
               .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.LastName)
               .NotEmpty().WithMessage("El apellido es requerido")
               .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            // Validar que la fecha pueda ser parseada desde el formato dd-MM-yyyy
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("La fecha es requerida")
                .Must(BeValidDateFormat).WithMessage("La fecha debe tener el formato dd-MM-yyyy")
            .Must(BeValidDate).WithMessage("La fecha no es válida");
            RuleFor(x => x.Email)
              .NotEmpty().WithMessage("El correo electrónico es requerido")
    .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido")
    .MaximumLength(255).WithMessage("El correo electrónico no puede exceder 255 caracteres")
    .MustAsync(async (email, cancellation) =>
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    })
    .WithMessage("El correo electrónico ya está registrado");

            RuleFor(RuleFor => RuleFor.Telephone)
                .MaximumLength(15).WithMessage("El teléfono no puede exceder 15 caracteres")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("El formato del teléfono no es válido");
        }

        private bool BeValidDateFormat(string fecha)
        {
            if (string.IsNullOrEmpty(fecha))
                return false;

            return DateTime.TryParseExact(fecha, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private bool BeValidDate(string fecha)
        {
            if (DateTime.TryParseExact(fecha, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result != default(DateTime) &&
                       result >= new DateTime(1900, 1, 1) &&
                       result <= new DateTime(2100, 12, 31);
            }
            return false;
        }
    }
}
