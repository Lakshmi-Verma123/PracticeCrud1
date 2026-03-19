using PracticeCrud1.Common;

namespace PracticeCrud1.Common
{
    public class AppResponse<T>
    {
        public StatusCode StatusCode { get; set; } = StatusCode.Error;
        public string Message { get; set; } = "Something went wrong";
        public T Result { get; set; }
    }

    public class AppResponse
    {
        public StatusCode StatusCode { get; set; } = StatusCode.Error;
        public string Message { get; set; } = "Something went wrong";
    }
}