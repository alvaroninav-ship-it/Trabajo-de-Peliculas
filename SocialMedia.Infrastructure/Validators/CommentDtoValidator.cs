using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.DTOs;

namespace Movies.Infrastructure.Validators
{
    public class CommentDtoValidator : AbstractValidator<CommentDto>
    {
        public readonly MoviesContext _context;
        public CommentDtoValidator(MoviesContext context)
        {
            _context = context;
            RuleFor(x => x.UserId)
               .GreaterThan(0)
               .WithMessage("El Id del User debe ser mayor que 0");

            RuleFor(x => x.ReviewId)
               .GreaterThan(0)
               .WithMessage("El Id de la review debe ser mayor que 0");

            RuleFor(x => x.Description)
               .NotEmpty().WithMessage("La descripción es requerida")
               .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
               .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres");

            // Validar que la fecha pueda ser parseada desde el formato dd-MM-yyyy
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("La fecha es requerida")
                .Must(BeValidDateFormat).WithMessage("La fecha debe tener el formato dd-MM-yyyy")
                .Must(BeValidDate).WithMessage("La fecha no es válida");
            RuleFor(x => x.ReviewId)
          .NotEmpty().WithMessage("El ID de la reseña es requerido")
          .MustAsync(async (reviewId, cancellation) =>
          {
              return await _context.Reviews.AnyAsync(r => r.Id == reviewId);
          })
          .WithMessage("La reseña especificada no existe");

            // Check that User exists
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El ID del usuario es requerido")
                .MustAsync(async (userId, cancellation) =>
                {
                    return await _context.Users.AnyAsync(u => u.Id == userId);
                })
                .WithMessage("El usuario especificado no existe");
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
