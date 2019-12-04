using System.Threading.Tasks;

/*
 * Version: 0.2.0
 * Author: Bartolomei Nicolo'
 * Date: 02/12/2019
 */

namespace DiabloBot
{
    class Program
    {
        public static Data data = new Data();

        //Thanks to:
        //Kapatona to help me find the function to log .png of item picked up
        static async Task Main(string[] args)
        {
            Gator gator = new Gator();
            await gator.ExecuterAsync();                       
        }                
    }
}
