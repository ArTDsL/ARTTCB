/**
 *
 * ART Tiny C Builder [ ARTTCB ]
 *
 * @file Logs.cs
 * @created 2026-02-10
 * @copyright Copyright (c) 2026. Arthur "ArTDsl" Dias dos Santos Lasso. All rights reserved.
 * @license MIT
 * 
 * Authors:
 *			Arthur "ArTDsl" Dias <arthur@artdslsoftwares.com.br>
 *
 * THE SOFTWARE IS PROVIDED  "AS IS",  WITHOUT WARRANTY OF ANY KIND,  EXPRESS OR
 * IMPLIED,  INCLUDING BUT NOT  LIMITED  TO  THE WARRANTIES  OF MERCHANTABILITY,
 * FITNESS FOR A  PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
 * AUTHORS  OR  COPYRIGHT HOLDERS BE  LIABLE  FOR ANY  CLAIM, DAMAGES  OR  OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ARTTCB{
	public enum ARTTCBLOGTYPE{
		ERROR,
		OK,
		INFO,
		WARNING,
		NONE
	}
	public class Log{
		public static FileStream file_stream;
		public static StreamWriter stream_writer;
		public bool CreateLogFile(string log_name){
			try{
				string logs_dir = $"{AppContext.BaseDirectory}\\ARTTCB_LOGS\\";
				if(!Directory.Exists(logs_dir)){
					Directory.CreateDirectory(logs_dir);
				}
				File.Create($"{logs_dir}{log_name}").Close();
				return true;
			}catch(Exception ex){
				Console.WriteLine($"{Texts.ARTTCB_LOG_STR}{Texts.ARTTCB_LOG_ERR} Could not create directory and/or log file because {ex.Message.ToString()}");
				return false;
			}
		}
		public static void AddToLog(string income_log_file, ARTTCBLOGTYPE type, string message, bool logInFile){
			string log_file = $"{AppContext.BaseDirectory}\\ARTTCB_LOGS\\{income_log_file}";
			DateTime dateTime = DateTime.Now;
			string actual_timestamp = dateTime.ToString("yyyy-MM-dd HH:mm:ss-ff");
			string log_info = $"{Texts.ARTTCB_LOG_STR}| {actual_timestamp} ] ";
			if(!File.Exists(log_file) && logInFile == true){
				Console.WriteLine($"{Texts.ARTTCB_LOG_STR}{Texts.ARTTCB_LOG_ERR} Cannot log ARTTCB progress because log folder/file does not exist...");
				return;	
			}
			switch(type){
				case ARTTCBLOGTYPE.ERROR: log_info += Texts.ARTTCB_LOG_ERR; break;
				case ARTTCBLOGTYPE.OK: log_info += Texts.ARTTCB_LOG_OK; break;
				case ARTTCBLOGTYPE.INFO: log_info += Texts.ARTTCB_LOG_INF; break;
				case ARTTCBLOGTYPE.WARNING: log_info += Texts.ARTTCB_LOG_WAR; break;
				case ARTTCBLOGTYPE.NONE: break;
			}
			string log_line = $"{log_info}:: {message}{Environment.NewLine}";
			Console.WriteLine(log_line);
			if(logInFile == true){
				using(stream_writer = new StreamWriter(log_file, append:true)){
					stream_writer.WriteLine(log_line);
					stream_writer.Flush();
					stream_writer.Dispose();
					stream_writer.Close();
				}
			}
			System.Threading.Thread.Sleep(50);
			return;
		}
	}
}
