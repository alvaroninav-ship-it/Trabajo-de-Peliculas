using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Movies.Core.CustomEntities;
using Movies.Core.Exceptions;

namespace Movies.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var path = context.HttpContext.Request.Path;

            // Determinar el código de estado HTTP
            var statusCode = GetStatusCode(exception);
            var errorResponse = CreateErrorResponse(exception, path, statusCode);

            // Loggear la excepción
            LogException(exception, statusCode, path);

            // Crear resultado
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }

        private int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                BusinessException businessEx => businessEx.StatusCode,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ArgumentException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                NotImplementedException => StatusCodes.Status501NotImplemented,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private ErrorResponse CreateErrorResponse(Exception exception, string path, int statusCode)
        {
            var errorResponse = new ErrorResponse
            {
                Type = exception.GetType().Name,
                Message = GetUserFriendlyMessage(exception, statusCode),
                Path = path
            };

            // Incluir ErrorCode si es una BusinessException
            if (exception is BusinessException businessEx && !string.IsNullOrEmpty(businessEx.ErrorCode))
            {
                errorResponse.ErrorCode = businessEx.ErrorCode;
            }

            return errorResponse;
        }

        private string GetUserFriendlyMessage(Exception exception, int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "Solicitud incorrecta. Verifique los datos enviados.",
                StatusCodes.Status401Unauthorized => "No autorizado. Debe autenticarse para acceder a este recurso.",
                StatusCodes.Status403Forbidden => "Acceso denegado. No tiene permisos para realizar esta acción.",
                StatusCodes.Status404NotFound => "Recurso no encontrado.",
                StatusCodes.Status500InternalServerError => "Error interno del servidor. Contacte al administrador.",
                _ => exception.Message // Para excepciones personalizadas, usar su mensaje
            };
        }

        private void LogException(Exception exception, int statusCode, string path)
        {
            var logMessage = $"Error {statusCode} en {path}: {exception.Message}";

            if (statusCode >= 500)
            {
                _logger.LogError(exception, logMessage);
            }
            else
            {
                _logger.LogWarning(exception, logMessage);
            }
        }
    }

}
