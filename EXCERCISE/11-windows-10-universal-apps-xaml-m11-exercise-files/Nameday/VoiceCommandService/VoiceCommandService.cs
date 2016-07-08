using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Common;

namespace VoiceCommandService
{
    public sealed class VoiceCommandService : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            taskInstance.Canceled += TaskInstance_Canceled;

            var triggerDetails = taskInstance.TriggerDetails
                as AppServiceTriggerDetails;

            if (triggerDetails?.Name != nameof(VoiceCommandService))
                return;

            var connection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

            connection.VoiceCommandCompleted += ConnectionOnVoiceCommandCompleted;

            var command = await connection.GetVoiceCommandAsync();
            //even if it's not used, you can run into issues without this line.

            switch (command.CommandName)
            {
                case "ReadTodaysNamedays":
                    await HandleReadNamedaysCommandAsync(connection);
                    break;
            }


            deferral.Complete();
        }

        private static async Task HandleReadNamedaysCommandAsync(VoiceCommandServiceConnection connection)
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Fetching today's namedays for you";
            userMessage.SpokenMessage = "Fetching today's namedays for you";
            var response = VoiceCommandResponse.CreateResponse(userMessage);
            await connection.ReportProgressAsync(response);

            var today = DateTime.Now.Date;
            var namedays = await NamedayRepository.GetAllNamedaysAsync();
            var todaysNameday = namedays.Find(e => e.Day == today.Day && e.Month == today.Month);
            var namedaysAsString = todaysNameday.NamesAsString;

            if (todaysNameday.Names.Count() == 1)
            {
                userMessage.SpokenMessage =
                    userMessage.DisplayMessage =
                    $"It is {namedaysAsString}'s nameday today";

                response = VoiceCommandResponse.CreateResponse(userMessage);
            }
            else
            {
                userMessage.SpokenMessage = $"Today's namedays are: {namedaysAsString}";
                userMessage.DisplayMessage = "Here are today's namedays:";

                var tile = new VoiceCommandContentTile();
                tile.ContentTileType = VoiceCommandContentTileType.TitleOnly;
                tile.Title = namedaysAsString;

                response = VoiceCommandResponse.CreateResponse(userMessage,
                    new List<VoiceCommandContentTile> { tile });
            }

            await connection.ReportSuccessAsync(response);
        }

        private void ConnectionOnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
        }
    }
}
