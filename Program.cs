
namespace Lotus {
	public class Program {

		[System.STAThread]
		public static void Main(string[] args) {

			using(var game = new Window()) {
				game.Run();
			}
		}
	}
}