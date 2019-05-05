namespace TankWebObserver.Utils
{
    public class PlayerFormatter : System.Net.Http.Formatting.MediaTypeFormatter
    {
        public PlayerFormatter()
        {
            SupportedMediaTypes.Add(new 
                System.Net.Http.Headers.MediaTypeHeaderValue(
                    "application/x-players"));
        }

        public override bool CanReadType(System.Type type)
        {
            return type == typeof(Models.Player) || 
                   type == typeof(System.Collections.Generic
                                  .List<Models.Player>);
        }

        public override bool CanWriteType(System.Type type)
        {
            return type == typeof(Models.Player) ||
                   type == typeof(System.Collections.Generic
                                  .List<Models.Player>);
        }

        public override async System.Threading.Tasks.Task WriteToStreamAsync(
            System.Type type, object value,
            System.IO.Stream writeStream, 
            System.Net.Http.HttpContent content,
            System.Net.TransportContext transportContext)
        {
            System.Collections.Generic.List<string> playersString = 
                new System.Collections.Generic.List<string>();
            System.Collections.Generic.IEnumerable<Models.Player> players =
                value is Models.Player ?
                new Models.Player[] { (Models.Player) value } :
                (System.Collections.Generic.IEnumerable<Models.Player>) value;
            foreach (var player in players)
            {
                playersString.Add(
                    string.Format(
                    "{0}|{1}|{2}|{3}|{4}|{5}", 
                    player.Id, player.Nickname, 
                    player.Score, player.Hp,
                    player.MaxHp, player.LeftCorner.ToString()));
            }

            System.IO.StreamWriter streamWriter = 
                new System.IO.StreamWriter(writeStream);
            await streamWriter.WriteAsync(
                string.Join("|", playersString));
            writeStream.Flush();
        }

    }
}