using PracticeCrud1.Common.DAL;
using PracticeCrud1.Models;

namespace PracticeCrud1.Common.Repository
{
    public class ContactService : IContactService
    {
        private readonly IDapperContext _dapperContext;

        public ContactService(IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<AppResponse<object>> SaveStudentData(SaveStudentData req)
        {
            AppResponse<object> result = new AppResponse<object>();
            try
            {
                var param = new
                {
                    Id = req.Id,
                    Name = req.Name,
                    Email = req.Email,
                    Number = req.Number,
                    Message = req.Message,
                    ImagePath = req.ImagePath,
                     ImageOriginalName = req.ImageOriginalName

                };
                result = await _dapperContext.QueryFirst<AppResponse<object>>("InsertInquiryData", param);
                if (result == null)
                {
                    result = new AppResponse<object>
                    {
                        StatusCode = 0,
                        Message = "Operation failed"
                    };
                }
            

            }
            catch (Exception ex)
            {
                result.StatusCode = 0;
                result.Message = "Server error";
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public async Task<IEnumerable<InquiryDetails>> GetInquiryDetails(int Id)
        {
            IEnumerable<InquiryDetails> list = new List<InquiryDetails>();
            try
            {
                var param = new
                {

                    Id = Id
                };
                list = await _dapperContext.QueryAll<InquiryDetails>("GetContactInquiry", param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
        }
        public async Task<AppResponse<object>> DeleteInquiryDetails(int Id)
        {
            AppResponse<object> result = new AppResponse<object>();
            try
            {
                var param = new
                {
                    Id = Id,
                };
                result = await _dapperContext.QueryFirst<AppResponse<object>>("DeleteInquiry", param);
                if (result == null)
                {
                    result = new AppResponse<object>
                    {
                        StatusCode = 0,
                        Message = "Delete failed"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.StatusCode = 0;
                result.Message = "Server error";
            }
            return result;
        }
        public async Task<SaveStudentData> GetStudentById(int Id)
        {
            SaveStudentData  data = null;
            try
            {
                var param = new { Id = Id };
                data = await _dapperContext.QueryFirst<SaveStudentData>("GetStudentById", param);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return data;
        }
        public async Task<IEnumerable<InquiryDetails>> GetAllInquiryDetails()
        {
            return await _dapperContext.QueryAll<InquiryDetails>("GetAllInquiryDetails", null);
        }

    }

}
