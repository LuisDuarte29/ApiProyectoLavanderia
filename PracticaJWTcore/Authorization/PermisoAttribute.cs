using Microsoft.AspNetCore.Mvc;

namespace PracticaJWTcore.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class PermisoAttribute : TypeFilterAttribute
    {
        public PermisoAttribute(string componente, string permiso)
            : base(typeof(PermisoFilter))
        {
            Arguments = [componente, permiso];
        }
    }
}
