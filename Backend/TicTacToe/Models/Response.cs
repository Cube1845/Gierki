namespace TicTacToe.Models
{
    public class Response
    {
        public string ResponseText { get; set; }
        public List<List<string>> Board { get; set; }
        public Response(string text, List<List<string>> board) 
        { 
            ResponseText = text;
            Board = board;
        }
        public Response(string text)
        {
            ResponseText = text;
            Board = null;
        }
    }
}
