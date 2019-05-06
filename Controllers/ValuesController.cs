using System.Web.Http;
using System.Threading;

using TankClient;
using TankCommon.Objects;
using TankWebObserver.Models;

namespace TankWebObserver.Controllers
{
   
    //localhost:12705/api/values
    public class ValuesController : ApiController
    {
        static WebSpectator _webSpectator;
        static bool isInitialized;
        Thread _clientThread;

        public ValuesController()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                var server = System.Configuration.ConfigurationManager.AppSettings["server"];
                var tokenSource = new CancellationTokenSource();
                ClientCore clientCore = new ClientCore(server, string.Empty);
                _webSpectator = new WebSpectator(tokenSource.Token);
                _clientThread = new Thread(() => {
                    clientCore.Run(false, _webSpectator.Client, tokenSource.Token);
                });
                _clientThread.Start();
            }
        }
        
        public string Get()
        {
            //Thread.Sleep(100);
            var map = _webSpectator?.Map;
            System.Collections.Generic.List<BaseInteractObject> mapObjects =
                _webSpectator?.Map?.InteractObjects;
            var players = new System.Collections.Generic.List<Player>();

            string response = string.Empty;
            if (map != null && 
                    map.Cells != null &&
                    map.InteractObjects != null)
            {
                response = ResponseString(map.Cells, mapObjects);
            }
            
            return response;
        }

        private string PlayersToString(System.Collections.Generic.List<Player> players)
        {
            var result = new System.Text.StringBuilder();
            if (players.Count > 0)
            {
                int i;
                int length = (players.Count - 1);
                for (i = 0; i < length; i++)
                {
                    result.Append(players[i].ToString());
                    result.Append('|');
                }
                result.Append(players[length].ToString());
            }
            return result.ToString();
        }

        private string InteractObjectsToString(System.Collections.Generic.List<InteractObject> interactObjects)
        {
            var result = new System.Text.StringBuilder();
            if (interactObjects.Count > 0)
            {
                int i, length = interactObjects.Count - 1;
                for (i = 0; i < length; i++)
                {
                    result.Append(interactObjects[i].ToString());
                    result.Append('|');
                }
                result.Append(interactObjects[i].ToString());
            }
            return result.ToString();
        }
        
        private string CellsToString(TankCommon.Enum.CellMapType[,] cells)
        {
            var result = new System.Text.StringBuilder();
            int r, c;
            int rowLength = cells.GetLength(0);
            int columnLength = cells.GetLength(1);
            result.AppendFormat("{0}|{1}", rowLength, columnLength);
            result.Append("+");
            if (rowLength > 0 && columnLength > 0)
            {
                for (r = 0; r < rowLength; r++)
                {
                    for (c = 0; c < columnLength; c++)
                    {
                        result.Append(((int)cells[r, c]).ToString());
                        result.Append('|');
                    }
                }
            }
            return result.ToString();
        }

        private string ResponseString(TankCommon.Enum.CellMapType[,] cells,
            System.Collections.Generic.List<BaseInteractObject> interactObjects)
        {
            var result = new System.Text.StringBuilder();
            int index = 0;
            var playersList = new System.Collections.Generic.List<Player>();
            var interactObjectsList = new System.Collections.Generic.List<InteractObject>();
            foreach (var obj in interactObjects)
            {
                if (obj is TankObject)
                {
                    var tank = obj as TankObject;
                    playersList.Add(
                        new Player(index, tank.Nickname, tank.Score,
                        tank.Hp, tank.MaximumHp, tank.Rectangle.LeftCorner));
                    ++index;
                }
                else
                {
                    string type = string.Empty;

                    if (obj is HealthUpgradeObject sobj)
                    {
                        type = "HealthUpgradeObject";
                    }
                    else if (obj is BulletSpeedUpgradeObject bsobj)
                    {
                        type = "BulletSpeedUpgradeObject";
                    }
                    else if (obj is DamageUpgradeObject duobj)
                    {
                        type = "DamageUpgradeObject";
                    }
                    else if (obj is SpeedUpgradeObject suobj)
                    {
                        type = "SpeedUpgradeObject";
                    }
                    else if (obj is MaxHpUpgradeObject mhuobj)
                    {
                        type = "MaxHpUpgradeObject";
                    }

                    interactObjectsList.Add(new InteractObject(type, obj.Rectangle.LeftCorner));
                }
            }

            result.Append(PlayersToString(playersList));
            result.Append("+");
            result.Append(CellsToString(cells));
            result.Append("+");
            result.Append(InteractObjectsToString(interactObjectsList));

            return result.ToString();
        }

        // POST api/values
        [HttpPost]
        public void CreatePlayer([FromBody]Player player)
        { 
        }

        // PUT api/values/5
        [HttpPut]
        public void EditPlayer(int id, [FromBody]Player value)
        {
        }

        // DELETE api/values/5
        [HttpDelete]
        public void DeletePlayer(int id)
        {
        }
        
    }
}
