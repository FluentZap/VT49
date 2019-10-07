using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace VT49
{

  enum ListOf_HolocronConnectionStatus
  {
    Connected,
    Connecting,
    Disconnected,
  }

  class VTHolocron
  {
    SWSimulation _sws = null;
    HubConnection connection;
    ListOf_HolocronConnectionStatus connectionStatus = ListOf_HolocronConnectionStatus.Disconnected;

    public VTHolocron(ref SWSimulation sws)
    {
      _sws = sws;




    }

    public void Update()
    {

    }

    public async void Init()
    {
      connection = new HubConnectionBuilder()
        .WithUrl("http://localhost:5000/Holocron")
        .WithAutomaticReconnect((new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) }))
        .Build();



      // connection.On<string, string>("ReceiveMessage", (user, message) =>);

      try
      {
        await connection.StartAsync();
        connectionStatus = ListOf_HolocronConnectionStatus.Connecting;
      }
      catch (Exception ex)
      {
        System.Console.WriteLine(ex.Message);
      }

      try
      {
        if (connection.State == HubConnectionState.Connected)
        {
          await connection.InvokeAsync("VT49Connect", "34f6d465a525aa589271e66648d73773");
        }
      }
      catch (Exception ex)
      {
        System.Console.WriteLine(ex.Message);
      }

      // await connection.InvokeAsync("SendMessage", userTextBox.Text, messageTextBox.Text);
      // connection.Closed += async (error) =>
      // {
      //   await Task.Delay(new Random().Next(0, 5) * 1000);
      //   await connection.StartAsync();
      // };

      connection.Reconnecting += error =>
      {
        // Debug.Assert(connection.State == HubConnectionState.Reconnecting);
        // Notify users the connection was lost and the client is reconnecting.
        // Start queuing or dropping messages.
        return Task.CompletedTask;
      };
    }
  }
}