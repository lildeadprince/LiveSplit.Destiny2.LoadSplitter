using System;
using System.Threading.Tasks;

namespace LiveSplit.Destiny2
{
    public enum LoadSplitterState
    {
        WaitingForDestinyProcess,
        WaitingForActivityStart,
        Idle
    }

    public class LoadSplitter
    {
        System.Diagnostics.Process d2Process = null;
        public event EventHandler OnDestinyActivityStart;
        public LoadSplitterState state { get; private set; }


        public LoadSplitter()
        {
        }


        public void StopWatching()
        {
            d2Process = null;
            state = LoadSplitterState.Idle;
        }

        public async Task StartWatchingApiStart()
        {
            state = LoadSplitterState.WaitingForDestinyProcess;

            try
            {
                d2Process = await FindProcess("destiny2");
                state = LoadSplitterState.WaitingForActivityStart;


                var targetProcessId = d2Process.Id;
                int targetPortRangeStart = 30000;
                int targetPortRangeEnd = 30009;
                int INTERVAL = 1000 / 30; // 30 per second

                while (state == LoadSplitterState.WaitingForActivityStart)
                {
                    var connections = IpHlpApi.IPHelper.GetTcpTable();
                    var fireteamActivityConnection = Array.Find(connections, c => c.SourceProcess == targetProcessId && targetPortRangeStart <= c.Remote.Port && c.Remote.Port <= targetPortRangeEnd);

                    if (fireteamActivityConnection != null)
                    {
                        OnDestinyActivityStart(this, null);
                        StopWatching();
                        break; // make sure of it
                    }

                    await Task.Delay(INTERVAL);
                }

            }
            catch (Exception e)
            {
                Options.Log.Error(e.Message + " " + e.StackTrace);
            }
        }

        private async Task<System.Diagnostics.Process> FindProcess(string processName)
        {
            System.Diagnostics.Process foundProcess = null;

            int INTERVAL = 10_000;

            while (state != LoadSplitterState.Idle && foundProcess == null)
            {
                var lookupProcess = System.Diagnostics.Process.GetProcessesByName(processName);

                #region Checks
                if (lookupProcess.Length == 0)
                {
                    Options.Log.Warning("Waiting for Destiny 2 process to start...");
                    await Task.Delay(INTERVAL);
                    continue;
                }

                if (lookupProcess.Length > 1)
                {
                    var errMsg = "Found more that one Destiny 2 process running. Behaviour undefined. Will not proceed further.";
                    Options.Log.Error(errMsg);
                    throw new Exception(errMsg);
                }
                #endregion

                // lookupProcess.Length == 1
                foundProcess = lookupProcess[0];
            }

            Options.Log.Info($"Destiny 2 Process <{foundProcess.Id}> found");

            return foundProcess;
        }
    }
}
