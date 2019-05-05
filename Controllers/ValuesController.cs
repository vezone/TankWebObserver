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
            System.Collections.Generic.List<BaseInteractObject> mapObjects =
                _webSpectator?.Map?.InteractObjects;
            var players = new System.Collections.Generic.List<Player>();

            if (mapObjects != null)
            {
                int index = 0;
                foreach (var obj in mapObjects)
                {
                    if (obj is TankObject)
                    {
                        var tank = obj as TankObject;
                        players.Add(
                            new Player(index, tank.Nickname, tank.Score,
                            tank.Hp, tank.MaximumHp, tank.Rectangle.LeftCorner));
                        ++index;
                    }
                }
            }
            else
            {
                players.Add(new Player(-1, "DefaultNone", 0, 0, 0, new Point(-1, -1)));
            }

            return ToHTMLString(players);
        }

        private string ToHTML(System.Collections.Generic.List<Player> players)
        {
            System.Text.StringBuilder html = new System.Text.StringBuilder();
            html.Append(
                "<table><th>Id</th><th>Nickname</th><th>Score</th><th>Hp</th><th>MaxHp</th><th>Tank position</th>");

            foreach (var player in players)
            {
                html.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}",  
                    "<tr>",
                    "<td>", player.Id, "</td>",
                    "<td>", player.Nickname, "</td>",
                    "<td>", player.Score, "</td>",
                    "<td>", player.Hp, "</td>",
                    "<td>", player.MaxHp, "</td>",
                    "<td>", player.LeftCorner.ToString(), "</td>",
                    "</tr>");
            }
            html.Append("</table>");
            return html.ToString();
        }

        private string ToHTMLString(System.Collections.Generic.List<Player> players)
        {
            string result = "";
            if (players.Count > 0)
            {
                int i;
                int length = (players.Count - 1);
                for (i = 0; i < length; i++)
                {
                    result += players[i].ToString() + "|";
                }
                result += players[length].ToString();
            }
            return result;
        }

        // GET api/values
        //public System.Collections.Generic.IEnumerable<Player> GetPlayers()
        //{
        //    return context;
        //}

        // GET api/values/5
        

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
