namespace PracticaJWTcore.Services
{
    // Resultado uniforme para que los services comuniquen exito o errores esperados sin lanzar excepciones.
    public class ServiceResult<T>
    {
        private ServiceResult(bool success, T? value, string? message, string? code)
        {
            Success = success;
            Value = value;
            Message = message;
            Code = code;
        }

        public bool Success { get; }
        public T? Value { get; }
        public string? Message { get; }
        public string? Code { get; }

        public static ServiceResult<T> Ok(T value) => new(true, value, null, null);

        public static ServiceResult<T> Fail(string message, string code) => new(false, default, message, code);
    }
}
