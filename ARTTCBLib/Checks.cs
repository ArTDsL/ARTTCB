/**
 *
 * ART Tiny C Builder [ ARTTCB ]
 *
 * @file Checks.cs
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
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace ARTTCB{
	public class Checks{
		private string log_file;
		private bool IsLogActive;
		public Checks(string _log_file, bool _IsLogActive){
			this.log_file = _log_file;
			this.IsLogActive = _IsLogActive;
		}
		public bool IsGCCInstalled(){
			string gcc_command;
			string gcc_args;
			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
				gcc_command = "where.exe";
				gcc_args = "gcc.exe";
			}else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)){
				gcc_command = "Command";
				gcc_args = "-v gcc";
			}else{
				Log.AddToLog(this.log_file, ARTTCBLOGTYPE.ERROR, "GCC is not installed in this computer.", this.IsLogActive);
				Environment.Exit(1);
				return false;// Apparently C# needs a f@#$% return in this case to not pop some wierd error since we are closing the application with System.Envirorment.Exit(1)...
			}
			var proc = new Process(){
				StartInfo = {
					FileName = gcc_command,
					Arguments = gcc_args,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};
			try{
				proc.Start();
				proc.WaitForExit();
				if(proc.ExitCode == 0){
					Log.AddToLog(this.log_file, ARTTCBLOGTYPE.OK, "GCC is installed.", this.IsLogActive);
					return true;
				}
			}catch(Exception){
				Log.AddToLog(this.log_file, ARTTCBLOGTYPE.ERROR, "GCC is not installed in this computer.", IsLogActive);
				Environment.Exit(1);
			}
			Log.AddToLog(this.log_file, ARTTCBLOGTYPE.ERROR, "GCC is not installed in this computer.", this.IsLogActive);
			Environment.Exit(1);
			return false; // Same as the first one, Dude... This code doesn't even run...
		}
		public string TCBFileExists(string tcb_file = null){
			string _tcb_file;
			_tcb_file = AppContext.BaseDirectory.ToString() + "/buildme.tcb";
			if(tcb_file != null){
				_tcb_file = AppContext.BaseDirectory.ToString() + "/" + tcb_file + ".tcb";
			}
			if(!File.Exists(_tcb_file)){
				Log.AddToLog(this.log_file, ARTTCBLOGTYPE.ERROR, "Buildme config file does not exist.", this.IsLogActive);
				Environment.Exit(1);
			}
			return _tcb_file;
		}
		public void IsOFileDirExists(string build_folder){
			if(!Directory.Exists(build_folder + "object\\")){
				Log.AddToLog(this.log_file, ARTTCBLOGTYPE.ERROR, "Something went wrong, directory \"object\" folder does not exist in your build location...", this.IsLogActive);
				Environment.Exit(1);
			}
			return;
		}
	}
}