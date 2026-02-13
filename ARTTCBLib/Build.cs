/**
 *
 * ART Tiny C Builder [ ARTTCB ]
 *
 * @file Build.cs
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Serialization;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace ARTTCB{
	public enum TCBTYPE{
		DLIB, // Dynamic Lib (0)
		SLIB, // Shared Lib (1)
		EXE, // Windows Executable (2)
		LEXEC, // Linux Executable (3)
		MEXEC // MacOS Executable (4)
	}
	public class TCBFile{
		public string project_name { get; set; }
		public string project_version { get; set; }
		public string project_author { get; set; }
		public string project_buildname {  get; set; }
		//Config
		public string working_dir { get; set; }
		public string build_folder { get; set; }
		//C Source & Headers (All folders in this section: Make sure this is inside working_dir, use "working_dir" as "ROOT")
		public string code_main_dir { get; set; }
		public string include_folders { get; set; }
		public TCBTYPE build_type { get; set; }
		public List<string> c_files { get; set; } // Inside "working_dir"/"code_main_dir".
		//Others
		public List<string> compiler_params { get; set; }
		//automatic argument definition
		/*
		 * TODO: Work on this later...	
		 */
		//continue build if needed
		public string next_build { get; set; }
		//options
		public bool auto_create_folders { get; set; }
		public bool generate_log { get; set; }
		public bool jump_object_compilation { get; set; }
	}
	public class Build{
		private string working_dir{ get; set; }
		private string object_dir { get; set; }
		private string build_dir { get; set; }
		private string source_dir { get; set; }
		private string includes_dir { get; set; }
		private TCBFile tcbInfo { get; set; }
		private string logfile_name { get; set; }
		private bool islog_active { get; set; }
		public Build(bool set_log_active){
			DateTime actual_dateTime = DateTime.Now;
			string logname = $"arttcb-log-{actual_dateTime.ToString("yyyy-MM-dd-HH-mm-ss-ff")}.log";
			Log log = new Log();
			log.CreateLogFile(logname);
			this.logfile_name = logname;
			this.islog_active = set_log_active; // Define Active using console for the beginning...
			Thread.Sleep(150);
			return;
		}
		public void BuildTCB(string buildme = null){
			//Building file should be defined as buildme.tcb or set in next_build for example "buildme2", file for buildme2 should use extension ".tcb" too.
			Checks checks = new Checks(this.logfile_name, this.islog_active);
			string builme_file;
			List<string> compiled_oFilesList = null;
			StringReader buildme_inputs;
			DeserializerBuilder buildme_des;
			TCBFile build_params;
			try{
				string _tcb_file = checks.TCBFileExists(buildme);
				builme_file = File.ReadAllText(_tcb_file);
				buildme_inputs = new StringReader(builme_file);
				buildme_des = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance);
				IDeserializer buildedme_des = buildme_des.Build();
				build_params = buildedme_des.Deserialize<TCBFile>(buildme_inputs);
				if(build_params.working_dir == null || build_params.working_dir == ""){
					build_params.working_dir = AppContext.BaseDirectory.ToString();
				}
				this.tcbInfo = build_params;
				this.working_dir = build_params.working_dir;
				this.build_dir = build_params.working_dir + build_params.build_folder;
				this.object_dir = this.build_dir + "object\\";
				this.source_dir = build_params.working_dir + build_params.code_main_dir;
				this.includes_dir = build_params.working_dir + build_params.include_folders;
				if(build_params.auto_create_folders == true){
					TCBCreateFolders();
				}
				checks.IsGCCInstalled();
				Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.NONE, "---> Project Info", (bool)this.tcbInfo.generate_log);
				Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.NONE, 
					$"\nProject Name: {this.tcbInfo.project_name}" +
					$"\nProject Version: {this.tcbInfo.project_version}" +
					$"\nProject Author: {this.tcbInfo.project_author}" +
					$"\nProject Build Name: {this.tcbInfo.project_buildname}", (bool)this.tcbInfo.generate_log);
				if(!build_params.jump_object_compilation){
					Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.NONE, "---> Building *.o files", (bool)this.tcbInfo.generate_log);
					compiled_oFilesList = BuildObjFiles();
				}
				switch(build_params.build_type){
					case TCBTYPE.DLIB:{
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.NONE, $"\n---> Building Dynamic Lib \"{this.tcbInfo.project_buildname}.dll\", \"lib{this.tcbInfo.project_buildname}.dll.a\" and \"lib{this.tcbInfo.project_buildname}.dll.def\"", (bool)this.tcbInfo.generate_log);
						BuildSharedLib(compiled_oFilesList);
						Console.WriteLine();
						break;
					}
					case TCBTYPE.EXE:{
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.NONE, $"\n---> Building Windows Executable (EXE) \"{this.tcbInfo.project_buildname}.exe\"", (bool)this.tcbInfo.generate_log);
						List<string> relative_OFiles = SearchRelativeOFiles();
						BuildWinEXE(relative_OFiles);
						Console.WriteLine();
						break;
					}
				}
				if(!String.IsNullOrEmpty(build_params.next_build)){
					BuildTCB(build_params.next_build);
				}
			}catch(Exception ex){
				Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.ERROR, $"ARTTCB Cannot build your project because {ex.Message}", (bool)this.tcbInfo.generate_log);
				return;
			}
		}
		public void TCBCreateFolders(){
			List<string> folder_array = new List<string>(){
				{this.object_dir},
				{this.build_dir},
			};
			if(!Directory.Exists(this.tcbInfo.working_dir)){
				Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.ERROR, $"The \"working_dir\" directory is invalid.", (bool)this.tcbInfo.generate_log);
				System.Environment.Exit(1);
			}
			foreach(string dir in folder_array){
				if(!Directory.Exists(dir)){
					Directory.CreateDirectory(dir);
				}
			}
			return;
		}
		public void ClearBuild(){
			DirectoryInfo dir = new DirectoryInfo(this.build_dir);
			DirectoryInfo odir = new DirectoryInfo(this.object_dir);
			foreach(FileInfo bFile in dir.GetFiles()){
				// Delete only if is a file
				if(File.Exists(this.build_dir + bFile)){
					bFile.Delete();
				}
			}
			//Do the same with .o files
			foreach(FileInfo oFile in odir.GetFiles()){
				// Delete only if is a file
				if(File.Exists(this.object_dir + oFile)){
					oFile.Delete();
				}
			}
			return;
		}
		public List<string> SearchRelativeOFiles(){
			//fetch only files from the specific build
			List<string> compiled_oFiles = new List<String>();
			foreach(string c_file in this.tcbInfo.c_files){
				string o_file = c_file.Replace(".c", ".o");
				compiled_oFiles.Add(o_file);
			}
			return compiled_oFiles;
		}
		public List<string> BuildObjFiles(){
			Checks checks = new Checks(this.logfile_name, this.tcbInfo.generate_log);
			string compiler_instructions;
			List<string> compiled_oFiles = new List<String>();
			checks.IsOFileDirExists(this.build_dir);
			foreach(var cFile in this.tcbInfo.c_files){
				string oFile = cFile.Replace(".c", ".o");
				//check if C file exists
				if(!File.Exists(this.source_dir + cFile)){
					Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.ERROR, $"Something went wrong, when building the object (*.o) files.", (bool)this.tcbInfo.generate_log);
					ClearBuild();
					System.Environment.Exit(1);
				}
				//Check if folder structure is the same in Object folder, otherwise create needed to ensure compilation (better organization)
				string compiler_args = "";
				foreach(var compiler_params in this.tcbInfo.compiler_params){
					compiler_args += compiler_params + " ";
				}
				string ofile_rebase = Path.GetFileName(oFile);
				compiler_instructions = $"-O2 -I {this.includes_dir} -c {this.source_dir}{cFile} -o {this.object_dir}{ofile_rebase} {compiler_args}";
				// Console.WriteLine(compiler_instructions); // Debug
				ExecuteBuildOnGCC(ofile_rebase, compiler_instructions);
				compiled_oFiles.Add(oFile);
			}
			return compiled_oFiles;
		}
		public void BuildSharedLib(List<string>o_files){
			string compiler_instructions;
			if(!Directory.Exists(this.object_dir)){
				Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.ERROR, $"Something went wrong, directory \"object\" folder does not exist in your build location...", (bool)this.tcbInfo.generate_log);
				System.Environment.Exit(1);
			}
			string ofiles_compiler = "";
			foreach(var oFile in o_files){
				//check if O file exists
				string ofile_rebase = Path.GetFileName(oFile);
				if(!File.Exists(this.object_dir + ofile_rebase)){
					Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.ERROR, $"Something went wrong, \"{ofile_rebase}\" is missing from \"object\" folder.", (bool)this.tcbInfo.generate_log);
					System.Environment.Exit(1);
				}
				ofiles_compiler += $"{this.object_dir}{ofile_rebase} ";
			}
			string compiler_args = "";
			foreach(var compiler_params in this.tcbInfo.compiler_params){
				compiler_args += $"{compiler_params} ";
			}
			compiler_instructions = $"-shared -o {this.build_dir}{this.tcbInfo.project_buildname}.dll {ofiles_compiler} -Wl,--out-implib,{this.build_dir}lib{this.tcbInfo.project_buildname}.dll.a -Wl,--output-def,{this.build_dir}lib{this.tcbInfo.project_buildname}.dll.def {compiler_args}";
			// Console.WriteLine(compile_args); // Debug
			ExecuteBuildOnGCC($"\"lib{this.tcbInfo.project_buildname}.dll\", \"lib{this.tcbInfo.project_buildname}.dll.a\" and \"lib{this.tcbInfo.project_buildname}.dll.def\"", compiler_instructions);
			return;
		}
		public void BuildWinEXE(List<string>o_files){
			string compiler_instructions;
			if(!Directory.Exists(this.object_dir)){
				Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.ERROR, $"Something went wrong, directory \"object\" folder does not exist in your build location...", (bool)this.tcbInfo.generate_log);
				System.Environment.Exit(1);
			}
			string ofiles_compiler = "";
			foreach(var oFile in o_files){
				//check if O file exists
				string ofile_rebase = Path.GetFileName(oFile);
				if(!File.Exists(this.object_dir + ofile_rebase)){
					Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.ERROR, $"Something went wrong, \"{ofile_rebase}\" is missing from \"object\" folder.", (bool)this.tcbInfo.generate_log);
					System.Environment.Exit(1);
				}
				ofiles_compiler += $"{this.object_dir}{ofile_rebase} ";
			}
			string compiler_args = "";
			foreach(var compiler_params in this.tcbInfo.compiler_params){
				compiler_args += $"{compiler_params} ";
			}
			compiler_instructions = $"-o {this.build_dir}{this.tcbInfo.project_buildname}.exe {ofiles_compiler} {compiler_args}";
			// Console.WriteLine(compile_args); // Debug
			ExecuteBuildOnGCC($"\"Windows Executable (EXE): {this.tcbInfo.project_buildname}.exe\"", compiler_instructions);
			return;
		}
		public void ExecuteBuildOnGCC(string output_name, string args){
			string output_buffer = "";
			string err_buffer = "";
			ProcessStartInfo procSTI = new ProcessStartInfo(){
				FileName = "gcc",
				Arguments = args,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			using(Process eproc = new Process(){ StartInfo = procSTI }){
				eproc.OutputDataReceived += (sender, e) => {
					if(e.Data != null){
						output_buffer += e.Data + "\n";
					}
				};
				eproc.ErrorDataReceived += (sender, e) => {
					if(e.Data != null){
						err_buffer += e.Data + "\n";
					}
				};
				try{
					eproc.Start();
					eproc.BeginOutputReadLine();
					eproc.BeginErrorReadLine();
					eproc.WaitForExit();
					if(!String.IsNullOrEmpty(output_buffer)){
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.INFO, $"---------- Building Debug ----------", (bool)this.tcbInfo.generate_log);
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.INFO, output_buffer.ToString(), (bool)this.tcbInfo.generate_log);
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.INFO, $"-------- EOF Building Debug --------", (bool)this.tcbInfo.generate_log);
					}
					if(!String.IsNullOrEmpty(err_buffer)){
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.INFO, $"----------- Errors Debug -----------", (bool)this.tcbInfo.generate_log);
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.INFO, err_buffer.ToString(), (bool)this.tcbInfo.generate_log);
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.INFO, $"--------- EOF Errors Debug ---------", (bool)this.tcbInfo.generate_log);
					}
					if(eproc.ExitCode == 0){
						Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.OK, $"\"{output_name}\" has been compiled with success.", (bool)this.tcbInfo.generate_log);
						return;
					}
				}catch(Exception){
					Log.AddToLog(this.logfile_name, ARTTCBLOGTYPE.ERROR, $"\"{output_name}\" failed to compile...", (bool)this.tcbInfo.generate_log);
					ClearBuild();
					System.Environment.Exit(1);
				}
			}
		}
	}
}
