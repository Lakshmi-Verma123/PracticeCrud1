using MediatR;
using PracticeCrud1.Common;

namespace PracticeCrud1.Models
{
    public class SaveStudentData
    {
        

        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public string Message { get; set; }
        public string ImagePath { get; set; }

        public string ImageOriginalName { get; set; }
        public IFormFile ImageFile { get; set; }
      


    }
    public class GetInquiryDetails : IRequest<IEnumerable<InquiryDetails>>
    {

    }
    public class InquiryDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public string Message { get; set; }
        public string ImagePath { get; set; }
        public string ImageOriginalName { get; set; }

    }

    public class DeleteInquiryDetails:IRequest<AppResponse<object>>
    {
        public int Id { get; set; }
    }
}