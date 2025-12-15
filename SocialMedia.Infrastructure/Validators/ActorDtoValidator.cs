using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.DTOs;

namespace Movies.Infrastructure.Validators
{
    public class ActorDtoValidator : AbstractValidator<ActorDto>
    {
        public readonly MoviesContext _context;

        public ActorDtoValidator(MoviesContext context)
        {
            _context = context;
            RuleFor(x => x.FirstName)
               .NotEmpty().WithMessage("El nombre es requerido")
               .MaximumLength(500).WithMessage("El nombre no puede exceder 500 caracteres");


            RuleFor(x => x.LastName)
               .NotEmpty().WithMessage("El apellido es requerido")
               .MaximumLength(500).WithMessage("El apellido no puede exceder 500 caracteres");
               
            
            // Validar que la fecha pueda ser parseada desde el formato dd-MM-yyyy
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("La fecha es requerida")
                .Must(BeValidDateFormat).WithMessage("La fecha debe tener el formato dd-MM-yyyy")
                .Must(BeValidDate).WithMessage("La fecha no es válida");

            RuleFor(x => x.MovieId)
           .NotEmpty().WithMessage("El ID de la película es requerido")
           .MustAsync(async (movieId, cancellation) =>
           {
               return await _context.Movies.AnyAsync(m => m.Id == movieId);
           })
           .WithMessage("La película especificada no existe");

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
