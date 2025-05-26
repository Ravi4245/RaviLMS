using RaviLMS.Models;

namespace RaviLMS.Repositories
{
    public interface ITeacherRepository
    {
        Task<bool> RegisterTeacherAsync(Teacher teacher);
        List<Teacher> GetApprovedTeachers();
    }
}

