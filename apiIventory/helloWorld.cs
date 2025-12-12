using DTO;
namespace apiIventory
{

    public class helloWorld : dto_helloWorld
    {
        public string sendMess(string message)
        {
            return "Gửi email: " + message;
        }
    }
}
