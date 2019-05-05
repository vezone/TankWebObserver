using System;
using System.Threading.Tasks;
using TankCommon;
using TankCommon.Enum;
using TankCommon.Objects;

namespace TankWebObserver.Models
{
    public class WebSpectator : TankClient.IClientBot
    {
        protected Map _map;
        protected DateTime _lastMapUpdate;
        protected readonly System.Threading.CancellationToken _cancellationToken;
        protected readonly object _syncObject = new object();
        protected int _msgCount;
        protected bool _wasUpdate;

        public Map Map => _map;

        public WebSpectator(System.Threading.CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
#pragma warning disable 4014
            UpdateMap();
#pragma warning restore 4014
        }

        public ServerResponse Client(int msgCount, ServerRequest request)
        {
            lock (_syncObject)
            {
                if (request.Map.Cells != null)
                {
                    _map = request.Map;
                    _lastMapUpdate = DateTime.Now;
                }
                else if (null == _map)
                {
                    return new ServerResponse { ClientCommand = ClientCommandType.UpdateMap };
                }
                
                _map.InteractObjects = request.Map.InteractObjects;
                _msgCount = msgCount;
                _wasUpdate = true;

                return new ServerResponse { ClientCommand = ClientCommandType.UpdateMap };
            }
        }

        protected async Task UpdateMap()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
                if (!_wasUpdate)
                {
                    continue;
                }
                
                lock (_syncObject)
                {
                    _wasUpdate = false;
                    _map = new Map(_map, _map.InteractObjects);
                }

            }

        }
    }
    

}