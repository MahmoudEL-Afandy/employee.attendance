namespace FGraduation_Project.DTO
{
    public class DeptDetailsWithEmpNameDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public List<string> EmpNames { get; set; }=new List<string>();


    }
}
