using FluentValidation;
using Movies.Contracts.Responses; 

namespace Movies.Api.Mapping
{
    public class ValidationMappingMiddleware
    {
        private readonly RequestDelegate next; 

        // The request delegate is method of the next middleware 
        // because this middleware is right before the controller, the next will be actually the controller call
        public ValidationMappingMiddleware(RequestDelegate next) 
        {
            this.next = next; 
        }

        // Each middleware in an API has to have the Invoke or InvokeAsync Method implemented
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // the next delegate is actually the controller 
                // so let the controller handle the request 
                // if the request fails because of validation, catch it and handle it
                await next(context); 
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = 400;

                var validationFailureResponse = new ValidationFailureResponse
                {
                    Errors = ex.Errors.Select(x => new ValidationResponse
                    {
                        PropertyName = x.PropertyName,
                        Message = x.ErrorMessage
                    }),
                };

                await context.Response.WriteAsJsonAsync(validationFailureResponse);
            }
        }
    }
}
