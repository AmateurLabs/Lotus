using System;

namespace Lotus {
	public class Program {

		[STAThread]
		public static void Main(string[] args) {

			using(var game = new Window()) {
				game.Run(60.0);
			}
		}
	}
}