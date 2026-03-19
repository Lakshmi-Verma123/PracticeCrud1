using PracticeCrud1.Models;

namespace PracticeCrud1.Common.Repository
{
    public interface IContactService

    {
        Task<AppResponse<object>> SaveStudentData(SaveStudentData req);
        Task<IEnumerable<InquiryDetails>> GetInquiryDetails(int Id);
        Task<AppResponse<object>> DeleteInquiryDetails(int Id);
        Task<SaveStudentData> GetStudentById(int Id);
        Task<IEnumerable<InquiryDetails>> GetAllInquiryDetails();


    }
}
