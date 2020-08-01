using System.Threading;
using System.Threading.Tasks;
using MarauderLib.Services;
using Newtonsoft.Json;

namespace MarauderLib.Objects {
  public class RunningTask
  {
    public RunningTask() {
      Id = CryptoService.GenerateSecureString(4);
      CancellationTokenSource = new CancellationTokenSource();
      
    }
    public string Id;
    public string TaskName;
    public string Command;
    [JsonIgnore]
    public Task Task;
    [JsonIgnore]
    public CancellationTokenSource CancellationTokenSource;
  }
}