namespace PracticaJWTcore.Models
{
    public class ComponentsForm
    {
        public int ComponentsId { get; set; }
        public string ComponentsName { get; set; }
        public List<RolesPermisos> RolesPermisos { get; set; }
    }
}
