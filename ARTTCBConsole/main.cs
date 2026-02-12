/**
 *
 * ART Tiny C Builder [ ARTTCB ] (Console)
 *
 * @file main.cs
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
using ARTTCB;
class ConsoleMain {  
	static int Main(string[] args){
		for(int i = 0; i < args.Length; i++){
			if(args.Length >= 1){
				if(args[i] == "build"){
					bool _log = false;
					if(args.Length >= 2 && args[(i + 1)] == "-log"){
						_log = true;
					}
					Build build = new Build(_log);
					build.BuildTCB();
				}
			}
		}
		return 0;
	}
}