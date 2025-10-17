using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Movies.Infrastructure.DTOs;

namespace Movies.Infrastructure.Validators
{
    public class MovieDtoValidator : AbstractValidator<MovieDto>
    {
        public MovieDtoValidator()
        {
            RuleFor(x => x.Description)
               .NotEmpty().WithMessage("La descripción es requerida")
               .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
               .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres");

            RuleFor(x => x.Title)
               .NotEmpty().WithMessage("El titulo es requerida")
               .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            // Validar que la fecha pueda ser parseada desde el formato dd-MM-yyyy
            RuleFor(x => x.ReleaseDate)
                .NotEmpty().WithMessage("La fecha es requerida")
                .Must(BeValidDateFormat).WithMessage("La fecha debe tener el formato dd-MM-yyyy")
                .Must(BeValidDate).WithMessage("La fecha no es válida");

            RuleFor(x => x.Length)
              .NotEmpty().WithMessage("La duración es requerida.")
               .Must(BeAValidDuration).WithMessage("Formato de duración no válido (ejemplo: '2h 15m').")
             .Must(BeWithinValidRange).WithMessage("La duración debe estar entre 1h y 4h.");

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
        private bool BeAValidDuration(string length)
        {
            return Regex.IsMatch(length, @"^\d{1,2}h(\s?\d{1,2}m)?$");
        }

        private bool BeWithinValidRange(string length)
        {
            var match = Regex.Match(length, @"(?<hours>\d{1,2})h(\s?(?<minutes>\d{1,2})m)?");
            if (!match.Success) return false;

            int hours = int.Parse(match.Groups["hours"].Value);
            int minutes = match.Groups["minutes"].Success ? int.Parse(match.Groups["minutes"].Value) : 0;

            int totalMinutes = hours * 60 + minutes;
            return totalMinutes >= 60 && totalMinutes <= 240;
        }
    }
}
